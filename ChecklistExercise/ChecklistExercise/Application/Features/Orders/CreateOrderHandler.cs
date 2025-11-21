using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ChecklistExercise.Application.Common.Logging;
using ChecklistExercise.Application.Features.Orders.Dtos;
using ChecklistExercise.Domain.Entities.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ChecklistExercise.Application.Features.Orders;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderProfileRequest, OrderProfileDto>
{
    private readonly ApplicationContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CreateOrderHandler> _logger;

    private const string AllOrdersCacheKey = "all_orders";

    public CreateOrderHandler(
        ApplicationContext db,
        IMapper mapper,
        IMemoryCache cache,
        ILogger<CreateOrderHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<OrderProfileDto> Handle(CreateOrderProfileRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var operationId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            request.Title,
            request.Author,
            request.ISBN,
            Category = request.Category.ToString(),
            OperationId = operationId
        };

        using var scope = _logger.BeginScope(new System.Collections.Generic.Dictionary<string, object?>
        {
            ["OperationId"] = operationId,
            ["ISBN"] = request.ISBN,
            ["Category"] = request.Category.ToString(),
            ["Title"] = request.Title
        });

        _logger.LogOrderEvent(LogEvents.OrderCreationStarted, LogLevel.Information,
            "Order creation started", payload);

        var swTotal = Stopwatch.StartNew();
        var swValidation = Stopwatch.StartNew();
        TimeSpan dbDuration = TimeSpan.Zero;

        try
        {
            _logger.LogOrderEvent(LogEvents.ISBNValidationPerformed, LogLevel.Debug,
                "Validating ISBN uniqueness", payload);

            bool isbnExists = await _db.Orders
                .AsNoTracking()
                .AnyAsync(o => o.ISBN == request.ISBN, ct);

            if (isbnExists)
            {
                swValidation.Stop();
                _logger.LogOrderEvent(LogEvents.OrderValidationFailed, LogLevel.Warning,
                    "Validation failed: ISBN already exists", payload);

                throw new ValidationException($"An order with ISBN '{request.ISBN}' already exists.");
            }

            _logger.LogOrderEvent(LogEvents.StockValidationPerformed, LogLevel.Debug,
                "Stock validation performed", new { request.StockQuantity, payload });

            swValidation.Stop();

            _logger.LogOrderEvent(LogEvents.DatabaseOperationStarted, LogLevel.Debug,
                "Database operation: Insert Order", payload);

            var swDb = Stopwatch.StartNew();

            var order = _mapper.Map<Order>(request);
            await _db.Orders.AddAsync(order, ct);
            await _db.SaveChangesAsync(ct);

            swDb.Stop();
            dbDuration = swDb.Elapsed;

            _logger.LogOrderEvent(LogEvents.DatabaseOperationCompleted, LogLevel.Information,
                "Database operation completed",
                new { payload, OrderId = order.Id, DbMs = dbDuration.TotalMilliseconds });

            _cache.Remove(AllOrdersCacheKey);
            _logger.LogOrderEvent(LogEvents.CacheOperationPerformed, LogLevel.Debug,
                "Cache invalidated", new { Key = AllOrdersCacheKey, payload });

            var dto = _mapper.Map<OrderProfileDto>(order);

            swTotal.Stop();
            _logger.LogOrderCreationMetrics(new OrderCreationMetrics(
                OperationId: operationId,
                OrderTitle: request.Title,
                ISBN: request.ISBN,
                Category: request.Category,
                ValidationDuration: swValidation.Elapsed,
                DatabaseSaveDuration: dbDuration,
                TotalDuration: swTotal.Elapsed,
                Success: true,
                ErrorReason: null
            ));

            return dto;
        }
        catch (ValidationException vex)
        {
            swTotal.Stop();

            _logger.LogOrderEvent(LogEvents.OrderValidationFailed, LogLevel.Warning,
                "Order creation validation error", payload, vex);

            _logger.LogOrderCreationMetrics(new OrderCreationMetrics(
                OperationId: operationId,
                OrderTitle: request.Title,
                ISBN: request.ISBN,
                Category: request.Category,
                ValidationDuration: swValidation.Elapsed,
                DatabaseSaveDuration: dbDuration,
                TotalDuration: swTotal.Elapsed,
                Success: false,
                ErrorReason: vex.Message
            ));
            throw;
        }
        catch (Exception ex)
        {
            swTotal.Stop();

            _logger.LogError(ex, "Unhandled error during order creation {@Payload}", payload);

            _logger.LogOrderCreationMetrics(new OrderCreationMetrics(
                OperationId: operationId,
                OrderTitle: request.Title,
                ISBN: request.ISBN,
                Category: request.Category,
                ValidationDuration: swValidation.Elapsed,
                DatabaseSaveDuration: dbDuration,
                TotalDuration: swTotal.Elapsed,
                Success: false,
                ErrorReason: ex.Message
            ));
            throw;
        }
    }
}

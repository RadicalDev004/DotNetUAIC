using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ChecklistExercise.Application.Common.Logging;
using ChecklistExercise.Application.Features.Orders;
using ChecklistExercise.Application.Features.Orders.Dtos;
using ChecklistExercise.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ChecklistExercise.Application.Features.Orders.Mapper;

namespace ChecklistExercise.Tests
{
    public sealed class CreateOrderHandlerIntegrationTests : IDisposable
    {
        private readonly ApplicationContext _db;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly Mock<ILogger<CreateOrderHandler>> _loggerMock;
        private readonly CreateOrderHandler _handler;

        public CreateOrderHandlerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase($"OrdersDbTests_{Guid.NewGuid():N}")
                .Options;

            _db = new ApplicationContext(options);
            _db.Database.EnsureCreated();


            var mapperCfg = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AdvancedOrderMappingProfile());
            });
            _mapper = mapperCfg.CreateMapper();

            _cache = new MemoryCache(new MemoryCacheOptions());
            _loggerMock = new Mock<ILogger<CreateOrderHandler>>();

            _handler = new CreateOrderHandler(_db, _mapper, _cache, _loggerMock.Object);
        }

        public void Dispose()
        {
            _db?.Dispose();
            (_cache as IDisposable)?.Dispose();
        }


        [Fact]
        public async Task Handle_ValidTechnicalOrderRequest_CreatesOrderWithCorrectMappings()
        {
            var req = new CreateOrderProfileRequest
            {
                Title = "Distributed Systems with .NET",
                Author = "Ada Lovelace",
                ISBN = "123-456-789-0123",
                Category = OrderCategory.Technical,
                Price = 120.50m,
                PublishedDate = DateTime.UtcNow.AddMonths(-6),
                CoverImageUrl = "https://example.com/cover.jpg",
                StockQuantity = 3
            };

            var dto = await _handler.Handle(req, CancellationToken.None);

            Assert.NotNull(dto);
            Assert.IsType<OrderProfileDto>(dto);

            Assert.Equal("Technical & Professional", dto.CategoryDisplayName);

            Assert.Equal("AL", dto.AuthorInitials);

            var currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            Assert.StartsWith(currency, dto.FormattedPrice);

            VerifyLogEventCalled(_loggerMock, LogEvents.OrderCreationStarted, Times.Once());
        }

        [Fact]
        public async Task Handle_DuplicateISBN_ThrowsValidationExceptionWithLogging()
        {
            var existing = new Order
            {
                Id = Guid.NewGuid(),
                Title = "Some Title",
                Author = "Some Author",
                ISBN = "999-999-999-9999",
                Category = OrderCategory.Fiction,
                Price = 10m,
                PublishedDate = DateTime.UtcNow.AddYears(-1),
                StockQuantity = 1,
            };
            _db.Orders.Add(existing);
            await _db.SaveChangesAsync();

            var req = new CreateOrderProfileRequest
            {
                Title = "Another Title",
                Author = "Another Author",
                ISBN = "999-999-999-9999",
                Category = OrderCategory.Technical,
                Price = 25m,
                PublishedDate = DateTime.UtcNow.AddMonths(-2),
                StockQuantity = 2
            };

            var ex = await Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(
                () => _handler.Handle(req, CancellationToken.None));

            Assert.Contains("already exists", ex.Message, StringComparison.OrdinalIgnoreCase);

            VerifyLogEventCalled(_loggerMock, LogEvents.OrderValidationFailed, Times.AtLeastOnce());
        }

        [Fact]
        public async Task Handle_ChildrensOrderRequest_AppliesDiscountAndConditionalMapping()
        {
            var req = new CreateOrderProfileRequest
            {
                Title = "Friendly Dragons",
                Author = "Jane Doe",
                ISBN = "111-222-333-4444",
                Category = OrderCategory.Children,
                Price = 40.00m,
                PublishedDate = DateTime.UtcNow.AddYears(-2),
                CoverImageUrl = "https://example.com/child.jpg",
                StockQuantity = 10
            };

            var dto = await _handler.Handle(req, CancellationToken.None);

            Assert.NotNull(dto);
            Assert.Equal("Children's Orders", dto.CategoryDisplayName);
            Assert.Equal(36.00m, dto.Price);
            Assert.Null(dto.CoverImageUrl);
        }


        private static void VerifyLogEventCalled(Mock<ILogger<CreateOrderHandler>> mock, int eventId, Times times)
        {
            mock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.Is<EventId>(e => e.Id == eventId),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }
    }
}

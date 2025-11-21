using ChecklistExercise.Application.Features.Orders;
using ChecklistExercise.Application.Features.Orders.Dtos;
using ChecklistExercise.Application.Features.Orders.Mapper;
using ChecklistExercise.Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ChecklistExercise.Application.Common.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orders API",
        Version = "v1",
        Description = "Order Management endpoints (creation, validation & telemetry)."
    });
});

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateOrderHandler>());

builder.Services.AddAutoMapper(cfg => { },
    typeof(AdvancedOrderMappingProfile).Assembly);


builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderProfileValidator>();
builder.Services.AddScoped<CreateOrderProfileValidator>();


builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationMiddleware>();


var orders = app.MapGroup("/orders").WithTags("Orders");


orders.MapPost("/", async (CreateOrderProfileRequest req, IMediator mediator, CreateOrderProfileValidator validator) =>
{
    var validationResult = await validator.ValidateAsync(req);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    var dto = await mediator.Send(req);
    return Results.Created($"/orders/{dto.Id}", dto);
})
.WithName("CreateOrder")
.WithOpenApi(op =>
{
    op.Summary = "Create a new order";
    op.Description = "Creates an order using advanced mapping, validation, logging, and telemetry.";
    return op;
});

app.Run();
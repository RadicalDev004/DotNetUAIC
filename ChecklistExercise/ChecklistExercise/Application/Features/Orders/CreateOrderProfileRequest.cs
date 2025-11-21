namespace ChecklistExercise.Application.Features.Orders;

using ChecklistExercise.Application.Features.Orders.Dtos;
using ChecklistExercise.Domain.Entities.Orders;
using MediatR;

public class CreateOrderProfileRequest : IRequest<OrderProfileDto>
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public OrderCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public int StockQuantity { get; set; } = 1;
}
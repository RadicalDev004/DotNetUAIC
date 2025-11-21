namespace ChecklistExercise.Application.Features.Orders.Mapper.Resolvers;
using AutoMapper;
using ChecklistExercise.Domain.Entities.Orders;
using ChecklistExercise.Application.Features.Orders.Dtos;

public class AvailabilityStatusResolver : IValueResolver<Order, OrderProfileDto, string>
{
    public string Resolve(Order src, OrderProfileDto dest, string destMember, ResolutionContext ctx)
    {
        if (!src.IsAvailable)
            return "Out of Stock";

        return src.StockQuantity switch
        {
            <= 0 => "Unavailable",
            1 => "LastCopy",
            <= 5 => "Limited Stock",
            _ => "In Stock"
        };
    }
}
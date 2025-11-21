namespace ChecklistExercise.Application.Features.Orders.Mapper.Resolvers;
using AutoMapper;
using ChecklistExercise.Domain.Entities.Orders;
using ChecklistExercise.Application.Features.Orders.Dtos;

public class CategoryDisplayResolver : IValueResolver<Order, OrderProfileDto, string>
{
    public string Resolve(Order src, OrderProfileDto dest, string destMember, ResolutionContext ctx)
    {
        return src.Category switch
        {
            OrderCategory.Fiction    => "Fiction & Literature",
            OrderCategory.NonFiction => "Non-Fiction",
            OrderCategory.Technical  => "Technical & Professional",
            OrderCategory.Children   => "Children's Orders",
            _                        => "Uncategorized"
        };
    }
}
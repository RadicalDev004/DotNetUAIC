namespace ChecklistExercise.Resolvers;
using AutoMapper;
using ChecklistExercise.Models;
using ChecklistExercise.Dtos;
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
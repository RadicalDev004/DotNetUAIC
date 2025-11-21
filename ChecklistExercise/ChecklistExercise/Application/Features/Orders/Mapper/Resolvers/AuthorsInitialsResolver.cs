namespace ChecklistExercise.Application.Features.Orders.Mapper.Resolvers;
using AutoMapper;
using ChecklistExercise.Application.Features.Orders.Dtos;
using ChecklistExercise.Domain.Entities.Orders;

public class AuthorInitialsResolver : IValueResolver<Order, OrderProfileDto, string>
{
    public string Resolve(Order src, OrderProfileDto dest, string destMember, ResolutionContext ctx)
    {
        if (string.IsNullOrWhiteSpace(src.Author)) return "?";

        var parts = src.Author
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        if (parts.Length == 1)
            return char.ToUpperInvariant(parts[0][0]).ToString();
        
        var first = parts.First()[0];
        var last = parts.Last()[0];
        return $"{char.ToUpperInvariant(first)}{char.ToUpperInvariant(last)}";
    }
}
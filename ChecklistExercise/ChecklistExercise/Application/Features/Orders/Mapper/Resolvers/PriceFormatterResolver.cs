namespace ChecklistExercise.Application.Features.Orders.Mapper.Resolvers;
using AutoMapper;
using ChecklistExercise.Domain.Entities.Orders;
using ChecklistExercise.Application.Features.Orders.Dtos;
using System.Globalization;

public class PriceFormatterResolver : IValueResolver<Order, OrderProfileDto, string>
{
    public string Resolve(Order src, OrderProfileDto dest, string destMember, ResolutionContext ctx)
        => src.Price.ToString("C2", CultureInfo.CurrentCulture);
}
using System.Globalization;
namespace ChecklistExercise.Resolvers;
using AutoMapper;
using ChecklistExercise.Models;
using ChecklistExercise.Dtos;


public class PriceFormatterResolver : IValueResolver<Order, OrderProfileDto, string>
{
    public string Resolve(Order src, OrderProfileDto dest, string destMember, ResolutionContext ctx)
        => src.Price.ToString("C2", CultureInfo.CurrentCulture);
}
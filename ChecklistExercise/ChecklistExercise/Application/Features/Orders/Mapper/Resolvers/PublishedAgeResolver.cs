namespace ChecklistExercise.Application.Features.Orders.Mapper.Resolvers;
using AutoMapper;
using ChecklistExercise.Application.Features.Orders.Dtos;
using ChecklistExercise.Domain.Entities.Orders;

public class PublishedAgeResolver : IValueResolver<Order, OrderProfileDto, string>
{
    public string Resolve(Order src, OrderProfileDto dest, string destMember, ResolutionContext ctx)
    {
        var nowUtc = DateTime.UtcNow;
        var publishedUtc = src.PublishedDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(src.PublishedDate, DateTimeKind.Utc)
            : src.PublishedDate.ToUniversalTime();

        if (publishedUtc > nowUtc)
            return "New Release";

        var days = (nowUtc - publishedUtc).TotalDays;

        switch (days)
        {
            case < 30:
                return "New Release";
            case < 365:
                var months = Math.Max(1, (int)Math.Floor(days / 30d));
                return $"{months} month{(months == 1 ? "" : "s")} old";
            case < 1825:
                var years = Math.Max(1, (int)Math.Floor(days / 365d));
                return $"{years} year{(years == 1 ? "" : "s")} old";
            default:
                return "Classic";
        }

    }
}
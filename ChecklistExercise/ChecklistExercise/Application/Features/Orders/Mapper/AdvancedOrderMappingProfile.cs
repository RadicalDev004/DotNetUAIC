using System;
using System.Globalization;
using AutoMapper;
using ChecklistExercise.Application.Features.Orders.Dtos;
using ChecklistExercise.Application.Features.Orders.Mapper.Resolvers;
using ChecklistExercise.Domain.Entities.Orders;

namespace ChecklistExercise.Application.Features.Orders.Mapper;

public class AdvancedOrderMappingProfile : Profile
{
    public AdvancedOrderMappingProfile()
    {
        CreateMap<CreateOrderProfileRequest, Order>()
            .ForMember(d => d.IsAvailable, opt => opt.Ignore());
        
        CreateMap<Order, OrderProfileDto>()
            .ForMember(d => d.CategoryDisplayName, opt => opt.MapFrom<CategoryDisplayResolver>())
            .ForMember(d => d.FormattedPrice, opt => opt.MapFrom<PriceFormatterResolver>())
            .ForMember(d => d.PublishedAge, opt => opt.MapFrom<PublishedAgeResolver>())
            .ForMember(d => d.AuthorInitials, opt => opt.MapFrom<AuthorInitialsResolver>())
            .ForMember(d => d.AvailabilityStatus, opt => opt.MapFrom<AvailabilityStatusResolver>())
            
            .ForMember(d => d.CoverImageUrl, opt => opt.MapFrom(src =>
                src.Category == OrderCategory.Children ? null : src.CoverImageUrl))
            .ForMember(d => d.Price, opt => opt.MapFrom(src =>
                src.Category == OrderCategory.Children
                    ? decimal.Round(src.Price * 0.9m, 2)
                    : src.Price))
            .ForMember(d => d.FormattedPrice, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.FormattedPrice = dest.Price.ToString("C2", CultureInfo.CurrentCulture);
            });
    }
}
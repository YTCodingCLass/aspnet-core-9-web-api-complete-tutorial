using AutoMapper;
using DependencyInjectionApi.Models;
using DependencyInjectionApi.Models.DTOs;

namespace DependencyInjectionApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.StockStatus,
                opt => opt.MapFrom(src => GetStockStatus(src.StockQuantity)));

        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, 
                opt => opt.Ignore());

        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, 
                opt => opt.Ignore());
    }

    private static string GetStockStatus(int stockQuantity)
    {
        return stockQuantity switch
        {
            0 => "Out of Stock",
            <= 10 => "Low Stock",
            _ => "In Stock"
        };
    }
}
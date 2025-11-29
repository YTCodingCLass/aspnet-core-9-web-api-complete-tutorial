using AutoMapper;
using ConfigurationOptionsApi.Models;
using ConfigurationOptionsApi.Models.DTOs;

namespace ConfigurationOptionsApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product to DTO
        CreateMap<Product, ProductResponseDto>();

        // DTO to Product
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Supplier to DTO
        CreateMap<Supplier, SupplierSummaryDto>();
    }
}

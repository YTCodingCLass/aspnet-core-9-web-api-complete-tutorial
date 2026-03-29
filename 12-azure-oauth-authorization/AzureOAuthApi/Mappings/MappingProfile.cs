using AutoMapper;
using AzureOAuthApi.Models;
using AzureOAuthApi.Models.DTOs;

namespace AzureOAuthApi.Mappings;

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

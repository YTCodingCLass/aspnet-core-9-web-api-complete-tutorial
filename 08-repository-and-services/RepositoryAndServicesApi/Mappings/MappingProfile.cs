using AutoMapper;
using RepositoryAndServicesApi.Models;
using RepositoryAndServicesApi.Models.DTOs;

namespace RepositoryAndServicesApi.Mappings;

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

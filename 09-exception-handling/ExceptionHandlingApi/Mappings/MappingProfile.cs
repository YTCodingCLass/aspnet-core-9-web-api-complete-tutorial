using AutoMapper;
using ExceptionHandlingApi.Models;
using ExceptionHandlingApi.Models.DTOs;

namespace ExceptionHandlingApi.Mappings;

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

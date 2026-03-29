// Services/IProductService.cs
using AzureOAuthApi.Models.DTOs;

namespace AzureOAuthApi.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<ProductResponseDto> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<ProductResponseDto>> GetLowStockProductsAsync();
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto);
    Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto updateDto);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
    Task<IEnumerable<ProductResponseDto>> BulkCreateProductsAsync(List<CreateProductDto> products);
}

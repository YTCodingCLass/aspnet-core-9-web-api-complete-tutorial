// Services/ProductService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using RepositoryAndServicesApi.Models;
using RepositoryAndServicesApi.Models.DTOs;
using RepositoryAndServicesApi.Repositories;

namespace RepositoryAndServicesApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        _logger.LogInformation(" Service: Getting all products");
        
        var products = await _productRepository.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        // Add business logic: calculate stock status
        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }
        
        _logger.LogInformation(" Service: Retrieved {Count} products", productDtos.Count());
        return productDtos;
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation(" Service: Getting product by ID: {ProductId}", id);
        
        var product = await _productRepository.GetByIdWithSupplierAsync(id);
        
        if (product == null)
        {
            _logger.LogWarning(" Service: Product not found: {ProductId}", id);
            return null;
        }
        
        var dto = _mapper.Map<ProductResponseDto>(product);
        dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        
        return dto;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category)
    {
        _logger.LogInformation(" Service: Getting products by category: {Category}", category);
        
        var products = await _productRepository.GetByCategoryAsync(category);
        var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }
        
        return productDtos;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetLowStockProductsAsync()
    {
        _logger.LogInformation(" Service: Getting low stock products");
        
        var products = await _productRepository.GetLowStockProductsAsync(10);
        var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }
        
        // Business logic: Send notification for critical stock
        var criticalStockProducts = productDtos.Where(p => p.StockQuantity == 0).ToList();
        if (criticalStockProducts.Any())
        {
            await _notificationService.SendLowStockAlertAsync(criticalStockProducts);
        }
        
        return productDtos;
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto)
    {
        _logger.LogInformation(" Service: Creating new product: {ProductName}", createDto.Name);
        
        // Business validation
        await ValidateProductCreationAsync(createDto);
        
        var product = _mapper.Map<Product>(createDto);
        var createdProduct = await _productRepository.CreateAsync(product);
        
        var responseDto = _mapper.Map<ProductResponseDto>(createdProduct);
        responseDto.StockStatus = CalculateStockStatus(responseDto.StockQuantity);
        
        // Business logic: Send notification for new product
        await _notificationService.SendProductCreatedNotificationAsync(responseDto);
        
        _logger.LogInformation(" Service: Product created successfully: {ProductId}", createdProduct.Id);
        return responseDto;
    }

    public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        _logger.LogInformation(" Service: Updating product: {ProductId}", id);
        
        // Check if product exists
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            _logger.LogWarning(" Service: Product not found for update: {ProductId}", id);
            return null;
        }
        
        // Business validation
        await ValidateProductUpdateAsync(id, updateDto);
        
        // Map and update
        _mapper.Map(updateDto, existingProduct);
        var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
        
        var responseDto = _mapper.Map<ProductResponseDto>(updatedProduct);
        responseDto.StockStatus = CalculateStockStatus(responseDto.StockQuantity);
        
        // Business logic: Check if stock changed significantly
        await CheckStockChangesAsync(existingProduct, updatedProduct);
        
        _logger.LogInformation(" Service: Product updated successfully: {ProductId}", id);
        return responseDto;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        _logger.LogInformation(" Service: Deleting product: {ProductId}", id);
        
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning(" Service: Product not found for deletion: {ProductId}", id);
            return false;
        }
        
        // Business logic: Check if product can be deleted
        await ValidateProductDeletionAsync(product);
        
        var result = await _productRepository.DeleteAsync(id);
        
        if (result)
        {
            // Business logic: Send deletion notification
            await _notificationService.SendProductDeletedNotificationAsync(product.Name);
            _logger.LogInformation(" Service: Product deleted successfully: {ProductId}", id);
        }
        
        return result;
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        return await _productRepository.ExistsAsync(id);
    }

    // Private business logic methods
    private string CalculateStockStatus(int stockQuantity)
    {
        return stockQuantity switch
        {
            0 => "Out of Stock",
            <= 5 => "Critical Low",
            <= 10 => "Low Stock",
            <= 50 => "Normal",
            _ => "Well Stocked"
        };
    }

    private async Task ValidateProductCreationAsync(CreateProductDto createDto)
    {
        // Business rule: Check if product name already exists
        var existingProducts = await _productRepository.GetAllAsync();
        if (existingProducts.Any(p => p.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new BusinessException($"Product with name '{createDto.Name}' already exists");
        }
        
        // Business rule: Validate profit margin
        if (createDto.Price <= createDto.CostPrice * 1.1m) // At least 10% markup
        {
            throw new BusinessException("Price must be at least 10% higher than cost price");
        }
    }

    private async Task ValidateProductUpdateAsync(int id, UpdateProductDto updateDto)
    {
        // Business rule: Check if another product has the same name
        var existingProducts = await _productRepository.GetAllAsync();
        if (existingProducts.Any(p => p.Id != id && p.Name.Equals(updateDto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new BusinessException($"Another product with name '{updateDto.Name}' already exists");
        }
    }

    private async Task ValidateProductDeletionAsync(Product product)
    {
        // Business rule: Don't delete products with high stock value
        var stockValue = product.Price * product.StockQuantity;
        if (stockValue > 10000m)
        {
            throw new BusinessException($"Cannot delete product with high stock value: ${stockValue:F2}");
        }
        
        await Task.CompletedTask; // Placeholder for async operations
    }

    private async Task CheckStockChangesAsync(Product original, Product updated)
    {
        var stockDifference = Math.Abs(original.StockQuantity - updated.StockQuantity);
        
        if (stockDifference > 10)
        {
            await _notificationService.SendStockChangeNotificationAsync(
                updated.Name, 
                original.StockQuantity, 
                updated.StockQuantity);
        }
    }
}

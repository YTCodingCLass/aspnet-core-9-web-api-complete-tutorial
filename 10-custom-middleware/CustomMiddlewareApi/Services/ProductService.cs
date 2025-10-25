// Services/ProductService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using CustomMiddlewareApi.Models;
using CustomMiddlewareApi.Models.DTOs;
using CustomMiddlewareApi.Repositories;
using CustomMiddlewareApi.Exceptions;

namespace CustomMiddlewareApi.Services;

public class ProductService(
    IProductRepository productRepository,
    INotificationService notificationService,
    IMapper mapper,
    ILogger<ProductService> logger)
    : IProductService
{
    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        logger.LogInformation(" Service: Getting all products");
        
        var products = await productRepository.GetAllAsync();
        var productDtos = mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        // Add business logic: calculate stock status
        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }
        
        logger.LogInformation(" Service: Retrieved {Count} products", productDtos.Count());
        return productDtos;
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(int id)
    {
        logger.LogInformation(" Service: Getting product by ID: {ProductId}", id);

        // Validation
        if (id <= 0)
        {
            throw new ValidationException("Id", "Product ID must be greater than zero");
        }

        var product = await productRepository.GetByIdWithSupplierAsync(id);

        if (product == null)
        {
            logger.LogWarning(" Service: Product not found: {ProductId}", id);
            throw new NotFoundException("Product", id);
        }

        var dto = mapper.Map<ProductResponseDto>(product);
        dto.StockStatus = CalculateStockStatus(dto.StockQuantity);

        return dto;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category)
    {
        logger.LogInformation(" Service: Getting products by category: {Category}", category);

        // Validation
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ValidationException("Category", "Category parameter is required");
        }

        if (category.Length < 2)
        {
            throw new ValidationException("Category", "Category must be at least 2 characters long");
        }

        var products = await productRepository.GetByCategoryAsync(category);
        var productDtos = mapper.Map<IEnumerable<ProductResponseDto>>(products);

        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }

        return productDtos;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetLowStockProductsAsync()
    {
        logger.LogInformation(" Service: Getting low stock products");
        
        var products = await productRepository.GetLowStockProductsAsync(10);
        var productDtos = mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }
        
        // Business logic: Send notification for critical stock
        var criticalStockProducts = productDtos.Where(p => p.StockQuantity == 0).ToList();
        if (criticalStockProducts.Any())
        {
            await notificationService.SendLowStockAlertAsync(criticalStockProducts);
        }
        
        return productDtos;
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto)
    {
        logger.LogInformation(" Service: Creating new product: {ProductName}", createDto.Name);
        
        // Business validation
        await ValidateProductCreationAsync(createDto);
        
        var product = mapper.Map<Product>(createDto);
        var createdProduct = await productRepository.CreateAsync(product);
        
        var responseDto = mapper.Map<ProductResponseDto>(createdProduct);
        responseDto.StockStatus = CalculateStockStatus(responseDto.StockQuantity);
        
        // Business logic: Send notification for new product
        await notificationService.SendProductCreatedNotificationAsync(responseDto);
        
        logger.LogInformation(" Service: Product created successfully: {ProductId}", createdProduct.Id);
        return responseDto;
    }

    public async Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        logger.LogInformation(" Service: Updating product: {ProductId}", id);

        // Check if product exists
        var existingProduct = await productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            logger.LogWarning(" Service: Product not found for update: {ProductId}", id);
            throw new NotFoundException("Product", id);
        }

        // Business validation
        await ValidateProductUpdateAsync(id, updateDto);

        // Map and update
        mapper.Map(updateDto, existingProduct);
        var updatedProduct = await productRepository.UpdateAsync(existingProduct);

        var responseDto = mapper.Map<ProductResponseDto>(updatedProduct);
        responseDto.StockStatus = CalculateStockStatus(responseDto.StockQuantity);

        // Business logic: Check if stock changed significantly
        if (updatedProduct != null)
            await CheckStockChangesAsync(existingProduct, updatedProduct);

        logger.LogInformation(" Service: Product updated successfully: {ProductId}", id);
        return responseDto;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        logger.LogInformation(" Service: Deleting product: {ProductId}", id);
        
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            logger.LogWarning(" Service: Product not found for deletion: {ProductId}", id);
            return false;
        }
        
        // Business logic: Check if product can be deleted
        await ValidateProductDeletionAsync(product);
        
        var result = await productRepository.DeleteAsync(id);
        
        if (result)
        {
            // Business logic: Send deletion notification
            await notificationService.SendProductDeletedNotificationAsync(product.Name);
            logger.LogInformation(" Service: Product deleted successfully: {ProductId}", id);
        }
        
        return result;
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        return await productRepository.ExistsAsync(id);
    }

    public async Task<IEnumerable<ProductResponseDto>> BulkCreateProductsAsync(List<CreateProductDto> products)
    {
        logger.LogInformation(" Service: Bulk creating {Count} products", products?.Count ?? 0);

        // Validation: Check if products list is valid
        var errors = new Dictionary<string, string[]>();

        if (products == null || !products.Any())
        {
            errors.Add("Products", ["At least one product must be provided"]);
        }
        else if (products.Count > 100)
        {
            errors.Add("Products", ["Cannot create more than 100 products at once"]);
        }

        // Check for duplicate names in the request
        if (products != null)
        {
            var duplicateNames = products
                .GroupBy(p => p.Name?.ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Any())
            {
                errors.Add("Products", [$"Duplicate product names found: {string.Join(", ", duplicateNames)}"]);
            }
        }

        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        // Create products
        var createdProducts = new List<ProductResponseDto>();
        foreach (var product in products!)
        {
            var createdProduct = await CreateProductAsync(product);
            createdProducts.Add(createdProduct);
        }

        logger.LogInformation(" Service: Successfully created {Count} products", createdProducts.Count);
        return createdProducts;
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
        var errors = new Dictionary<string, string[]>();

        // Validation: Product name
        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            errors.Add("Name", ["Product name is required"]);
        }
        else if (createDto.Name.Length < 3)
        {
            errors.Add("Name", ["Product name must be at least 3 characters long"]);
        }
        else if (createDto.Name.Length > 100)
        {
            errors.Add("Name", ["Product name cannot exceed 100 characters"]);
        }

        // Validation: Price
        if (createDto.Price <= 0)
        {
            errors.Add("Price", ["Price must be greater than zero"]);
        }
        else if (createDto.Price > 1000000)
        {
            errors.Add("Price", ["Price cannot exceed 1,000,000"]);
        }

        // Validation: Cost Price
        if (createDto.CostPrice < 0)
        {
            errors.Add("CostPrice", ["Cost price cannot be negative"]);
        }

        // Validation: Stock Quantity
        if (createDto.StockQuantity < 0)
        {
            errors.Add("StockQuantity", ["Stock quantity cannot be negative"]);
        }
        else if (createDto.StockQuantity > 100000)
        {
            errors.Add("StockQuantity", ["Stock quantity cannot exceed 100,000"]);
        }

        // Validation: Category
        if (string.IsNullOrWhiteSpace(createDto.Category))
        {
            errors.Add("Category", ["Category is required"]);
        }

        // Throw ValidationException if there are any validation errors
        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        // Business rule: Check if product name already exists
        var existingProducts = await productRepository.GetAllAsync();
        if (existingProducts.Any(p => p.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException($"Product with name '{createDto.Name}' already exists");
        }

        // Business rule: Validate profit margin
        if (createDto.Price <= createDto.CostPrice * 1.1m) // At least 10% markup
        {
            throw new BadRequestException("Price must be at least 10% higher than cost price");
        }
    }

    private async Task ValidateProductUpdateAsync(int id, UpdateProductDto updateDto)
    {
        var errors = new Dictionary<string, string[]>();

        // Validation: Product name
        if (string.IsNullOrWhiteSpace(updateDto.Name))
        {
            errors.Add("Name", ["Product name is required"]);
        }
        else if (updateDto.Name.Length < 3)
        {
            errors.Add("Name", ["Product name must be at least 3 characters long"]);
        }
        else if (updateDto.Name.Length > 100)
        {
            errors.Add("Name", ["Product name cannot exceed 100 characters"]);
        }

        // Validation: Price
        if (updateDto.Price <= 0)
        {
            errors.Add("Price", ["Price must be greater than zero"]);
        }
        else if (updateDto.Price > 1000000)
        {
            errors.Add("Price", ["Price cannot exceed 1,000,000"]);
        }

        // Validation: Stock Quantity
        if (updateDto.StockQuantity < 0)
        {
            errors.Add("StockQuantity", ["Stock quantity cannot be negative"]);
        }
        else if (updateDto.StockQuantity > 100000)
        {
            errors.Add("StockQuantity", ["Stock quantity cannot exceed 100,000"]);
        }

        // Validation: Category
        if (string.IsNullOrWhiteSpace(updateDto.Category))
        {
            errors.Add("Category", ["Category is required"]);
        }

        // Throw ValidationException if there are any validation errors
        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        // Business rule: Check if another product has the same name
        var existingProducts = await productRepository.GetAllAsync();
        if (existingProducts.Any(p => p.Id != id && p.Name.Equals(updateDto.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException($"Another product with name '{updateDto.Name}' already exists");
        }
    }

    private async Task ValidateProductDeletionAsync(Product product)
    {
        // Business rule: Don't delete products with high stock value
        var stockValue = product.Price * product.StockQuantity;
        if (stockValue > 10000m)
        {
            throw new BadRequestException($"Cannot delete product with high stock value: ${stockValue:F2}");
        }

        await Task.CompletedTask; // Placeholder for async operations
    }

    private async Task CheckStockChangesAsync(Product original, Product updated)
    {
        var stockDifference = Math.Abs(original.StockQuantity - updated.StockQuantity);
        
        if (stockDifference > 10)
        {
            await notificationService.SendStockChangeNotificationAsync(
                updated.Name, 
                original.StockQuantity, 
                updated.StockQuantity);
        }
    }
}

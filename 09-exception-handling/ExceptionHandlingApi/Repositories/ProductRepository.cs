// Repositories/ProductRepository.cs
using Microsoft.Extensions.Logging;
using ExceptionHandlingApi.Data;
using ExceptionHandlingApi.Models;

namespace ExceptionHandlingApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ILogger<ProductRepository> logger)
    {
        _logger = logger;
        // Initialize suppliers for products
        InMemoryDatabase.SeedProductSuppliers();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        _logger.LogInformation(" Repository: Getting all products");
        
        // Simulate async operation
        await Task.Delay(10);
        
        return InMemoryDatabase.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToList();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        _logger.LogInformation(" Repository: Getting product by ID: {ProductId}", id);
        
        await Task.Delay(10);
        
        return InMemoryDatabase.Products
            .FirstOrDefault(p => p.Id == id && p.IsActive);
    }

    public async Task<Product?> GetByIdWithSupplierAsync(int id)
    {
        _logger.LogInformation(" Repository: Getting product with supplier by ID: {ProductId}", id);
        
        await Task.Delay(10);
        
        var product = InMemoryDatabase.Products
            .FirstOrDefault(p => p.Id == id && p.IsActive);

        if (product?.SupplierId.HasValue == true)
        {
            product.Supplier = InMemoryDatabase.Suppliers
                .FirstOrDefault(s => s.Id == product.SupplierId);
        }

        return product;
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        _logger.LogInformation(" Repository: Getting products by category: {Category}", category);
        
        await Task.Delay(10);
        
        return InMemoryDatabase.Products
            .Where(p => p.IsActive && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Name)
            .ToList();
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
    {
        _logger.LogInformation(" Repository: Getting low stock products with threshold: {Threshold}", threshold);
        
        await Task.Delay(10);
        
        return InMemoryDatabase.Products
            .Where(p => p.IsActive && p.StockQuantity <= threshold)
            .OrderBy(p => p.StockQuantity)
            .ToList();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _logger.LogInformation(" Repository: Creating new product: {ProductName}", product.Name);
        
        await Task.Delay(10);
        
        product.Id = await GetNextIdAsync();
        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;
        
        InMemoryDatabase.Products.Add(product);
        
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        _logger.LogInformation(" Repository: Updating product: {ProductId}", product.Id);
        
        await Task.Delay(10);
        
        var existingProduct = InMemoryDatabase.Products
            .FirstOrDefault(p => p.Id == product.Id && p.IsActive);

        if (existingProduct == null)
            return null;

        // Update properties
        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Category = product.Category;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.SupplierId = product.SupplierId;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation(" Repository: Soft deleting product: {ProductId}", id);
        
        await Task.Delay(10);
        
        var product = InMemoryDatabase.Products
            .FirstOrDefault(p => p.Id == id && p.IsActive);

        if (product == null)
            return false;

        // Soft delete
        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        await Task.Delay(5);
        
        return InMemoryDatabase.Products
            .Any(p => p.Id == id && p.IsActive);
    }

    public async Task<int> GetNextIdAsync()
    {
        await Task.Delay(5);
        
        return InMemoryDatabase.Products.Any() 
            ? InMemoryDatabase.Products.Max(p => p.Id) + 1 
            : 1;
    }
}
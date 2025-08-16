namespace DependencyInjectionApi.Models.DTOs;

public class ProductResponseDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    /// <example>Gaming Laptop</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    /// <example>999.99</example>
    public decimal Price { get; set; }

    /// <summary>
    /// Product category
    /// </summary>
    /// <example>Electronics</example>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Creation date
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Stock quantity
    /// </summary>
    /// <example>50</example>
    public int StockQuantity { get; set; }
    
    /// <summary>
    /// Stock status based on quantity
    /// </summary>
    /// <example>In Stock</example>
    public string StockStatus { get; set; } = string.Empty;
    
    // Flattened supplier info (if exists)
    /// <summary>
    /// Supplier company name (if available)
    /// </summary>
    /// <example>Tech Supplies Inc.</example>
    public string? SupplierCompanyName { get; set; }
    
    /// <summary>
    /// Supplier contact name (if available)
    /// </summary>
    /// <example>John Doe</example>
    public string? SupplierContactName { get; set; }
}
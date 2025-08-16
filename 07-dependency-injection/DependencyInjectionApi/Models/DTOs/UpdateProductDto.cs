using System.ComponentModel.DataAnnotations;

namespace DependencyInjectionApi.Models.DTOs;

/// <summary>
/// DTO for updating a product (PUT - complete replacement)
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// Product name
    /// </summary>
    /// <example>Updated Gaming Mouse</example>
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    /// <example>59.99</example>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// Product category
    /// </summary>
    /// <example>Gaming</example>
    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 50 characters")]
    public string Category { get; set; } = string.Empty;
    
    
    /// <summary>
    /// Stock quantity
    /// </summary>
    /// <example>75</example>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }
}
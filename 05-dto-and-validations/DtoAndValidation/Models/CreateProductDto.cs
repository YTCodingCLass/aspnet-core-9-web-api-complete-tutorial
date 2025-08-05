using System.ComponentModel.DataAnnotations;

namespace DtoAndValidation.Models;

/// <summary>
/// DTO for creating a new product
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// Product name
    /// </summary>
    /// <example>Gaming Laptop</example>
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    /// <example>999.99</example>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// Product category
    /// </summary>
    /// <example>Electronics</example>
    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 50 characters")]
    public string Category { get; set; } = string.Empty;
}
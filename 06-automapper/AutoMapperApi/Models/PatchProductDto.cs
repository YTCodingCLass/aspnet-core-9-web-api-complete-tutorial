using System.ComponentModel.DataAnnotations;

namespace AutoMapperApi.Models;

/// <summary>
/// DTO for partially updating a product (PATCH - partial update)
/// </summary>
public class PatchProductDto
{
    /// <summary>
    /// Product name (optional for patch)
    /// </summary>
    /// <example>Partially Updated Mouse</example>
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Product price (optional for patch)
    /// </summary>
    /// <example>39.99</example>
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Product category (optional for patch)
    /// </summary>
    /// <example>Accessories</example>
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 50 characters")]
    public string? Category { get; set; }
}
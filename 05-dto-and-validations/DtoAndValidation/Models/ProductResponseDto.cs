namespace DtoAndValidation.Models;

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

}
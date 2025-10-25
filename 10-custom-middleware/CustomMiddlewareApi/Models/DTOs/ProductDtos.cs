// Models/DTOs/ProductDtos.cs
using System.ComponentModel.DataAnnotations;

namespace CustomMiddlewareApi.Models.DTOs;

public class CreateProductDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public int? SupplierId { get; set; }
    
    [Range(0.01, 999999.99)]
    public decimal CostPrice { get; set; }
}

public class UpdateProductDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public int? SupplierId { get; set; }
}

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string StockStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public SupplierSummaryDto? Supplier { get; set; }
}

public class SupplierSummaryDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
}

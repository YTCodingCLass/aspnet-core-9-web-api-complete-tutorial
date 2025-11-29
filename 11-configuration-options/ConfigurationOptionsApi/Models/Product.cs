// Models/Product.cs
namespace ConfigurationOptionsApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    
    // Internal properties
    public decimal CostPrice { get; set; }
    public string InternalNotes { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
}

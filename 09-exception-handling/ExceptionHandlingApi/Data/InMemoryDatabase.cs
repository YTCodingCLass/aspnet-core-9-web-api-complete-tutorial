// Data/InMemoryDatabase.cs
using ExceptionHandlingApi.Models;

namespace ExceptionHandlingApi.Data;

public static class InMemoryDatabase
{
    private static readonly List<Supplier> _suppliers = new()
    {
        new Supplier 
        { 
            Id = 1, 
            CompanyName = "Tech Supplies Inc.", 
            ContactName = "John Doe",
            Phone = "555-0100",
            Email = "john@techsupplies.com"
        },
        new Supplier 
        { 
            Id = 2, 
            CompanyName = "Peripheral World", 
            ContactName = "Jane Smith",
            Phone = "555-0200",
            Email = "jane@peripheralworld.com"
        },
        new Supplier 
        { 
            Id = 3, 
            CompanyName = "Electronics Hub", 
            ContactName = "Mike Johnson",
            Phone = "555-0300",
            Email = "mike@electronicshub.com"
        }
    };

    private static readonly List<Product> _products = new()
    {
        new Product 
        { 
            Id = 1, 
            Name = "Gaming Laptop", 
            Price = 1499.99m, 
            Category = "Electronics",
            StockQuantity = 25,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            SupplierId = 1,
            CostPrice = 1200.00m,
            CreatedByUserId = 1
        },
        new Product 
        { 
            Id = 2, 
            Name = "Wireless Mouse", 
            Price = 49.99m, 
            Category = "Accessories",
            StockQuantity = 8,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            SupplierId = 2,
            CostPrice = 30.00m,
            CreatedByUserId = 1
        },
        new Product 
        { 
            Id = 3, 
            Name = "Mechanical Keyboard", 
            Price = 129.99m, 
            Category = "Accessories",
            StockQuantity = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            SupplierId = 2,
            CostPrice = 85.00m,
            CreatedByUserId = 2
        },
        new Product 
        { 
            Id = 4, 
            Name = "4K Monitor", 
            Price = 599.99m, 
            Category = "Displays",
            StockQuantity = 15,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            SupplierId = 3,
            CostPrice = 450.00m,
            CreatedByUserId = 1
        }
    };

    public static List<Product> Products => _products;
    public static List<Supplier> Suppliers => _suppliers;

    public static void SeedProductSuppliers()
    {
        foreach (var product in _products)
        {
            if (product.SupplierId.HasValue)
            {
                product.Supplier = _suppliers.FirstOrDefault(s => s.Id == product.SupplierId);
            }
        }
    }
}
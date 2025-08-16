using DependencyInjectionApi.Models.DTOs;

namespace DependencyInjectionApi.Models;

/// <summary>
/// Represents a product entity in the system
/// Contains both public-facing properties and internal management fields
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product
    /// Primary key in the database
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Display name of the product
    /// Used for customer-facing display and search
    /// </summary>
    /// <example>Gaming Laptop</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Selling price of the product in decimal format
    /// This is the price shown to customers
    /// </summary>
    /// <example>999.99</example>
    public decimal Price { get; set; }

    /// <summary>
    /// Category classification for the product
    /// Used for organization and filtering
    /// </summary>
    /// <example>Electronics</example>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the product was first created in the system
    /// Used for auditing and reporting purposes
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; }
    
    
    // Internal management properties - not exposed to public APIs
    
    /// <summary>
    /// Internal notes about the product for staff use only
    /// Contains sensitive information not meant for customers
    /// </summary>
    public string InternalNotes { get; set; }   
    
    /// <summary>
    /// Soft delete flag to mark products as deleted without removing from database
    /// Allows for data recovery and maintains referential integrity
    /// </summary>
    public bool IsDeleted { get; set; }          
    
    /// <summary>
    /// ID of the user who created this product record
    /// Used for auditing and accountability purposes
    /// </summary>
    public int CreatedByUserId { get; set; } 
    
    /// <summary>
    /// Internal cost price of the product (what the company pays)
    /// Used for profit margin calculations, not shown to customers
    /// </summary>
    public decimal CostPrice { get; set; }
    
    
    /// <summary>
    /// Current quantity of the product available in inventory
    /// Used for stock management and availability checks
    /// </summary>
    public int StockQuantity { get; set; }
    
    /// <summary>
    /// Navigation property to the supplier of this product
    /// Represents a one-to-many relationship (Supplier -> Products)
    /// Nullable as products might not have an assigned supplier
    /// </summary>
    public Supplier? Supplier { get; set; }
    
    /// <summary>
    /// Manual mapping method to convert Product entity to ProductResponseDto
    /// This demonstrates manual object mapping before using AutoMapper
    /// Only maps public-facing properties, excludes sensitive internal data
    /// </summary>
    /// <returns>ProductResponseDto with public properties mapped</returns>
    public ProductResponseDto MapToResponseDto()
    {
        return new ProductResponseDto()
        {
            Id = this.Id,
            Name = this.Name,
            Price = this.Price,
            Category = this.Category,
            CreatedAt = this.CreatedAt
        };
    }
}

/// <summary>
/// Represents a supplier entity that provides products
/// Contains basic contact and company information
/// </summary>
public class Supplier
{
    /// <summary>
    /// Unique identifier for the supplier
    /// Primary key in the database
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Official name of the supplier company
    /// Used for business correspondence and records
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the primary contact person at the supplier company
    /// Used for direct communication
    /// </summary>
    public string ContactName { get; set; } = string.Empty;
    
    /// <summary>
    /// Phone number for contacting the supplier
    /// Primary communication method for orders and inquiries
    /// </summary>
    public string Phone { get; set; } = string.Empty;
}
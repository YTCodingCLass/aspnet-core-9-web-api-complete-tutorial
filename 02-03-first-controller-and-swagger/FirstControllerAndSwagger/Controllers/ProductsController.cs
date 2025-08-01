using FirstControllerAndSwagger.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstControllerAndSwagger.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ILogger<ProductsController> logger) : ControllerBase
{
    /// <summary>
    /// Gets all Products
    /// </summary>
    /// <returns>List of Products</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetProduct()
    {
        logger.LogInformation("Getting all Products");

        var products = GetSampleProducts();

        return Ok(products);
    }
    
    /// <summary>
    /// Gets product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>A single product</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetProductById(int id)
    {
        var product = GetSampleProducts().FirstOrDefault(x => x.Id == id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
    
    private static IEnumerable<Product> GetSampleProducts()
    {
        return new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { Id = 2, Name = "Mouse", Price = 29.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 3, Name = "Keyboard", Price = 79.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new() { Id = 4, Name = "Monitor", Price = 299.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-1) }
        };
    }
}
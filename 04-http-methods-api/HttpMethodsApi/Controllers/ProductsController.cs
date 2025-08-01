using HttpMethodsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HttpMethodsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ILogger<ProductsController> logger) : ControllerBase
{
    private static List<Product> products = new()
    {
        new() { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-10) },
        new() { Id = 2, Name = "Mouse", Price = 29.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new() { Id = 3, Name = "Keyboard", Price = 79.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-2) },
        new() { Id = 4, Name = "Monitor", Price = 299.99m, Category = "Electronics", CreatedAt = DateTime.UtcNow.AddDays(-1) }
    };
    
    /// <summary>
    /// Gets all Products
    /// </summary>
    /// <returns>List of Products</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetProduct()
    {
        logger.LogInformation("Getting all Products");

        var products = ProductsController.products;

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
        var product = products.FirstOrDefault(x => x.Id == id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
    
    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">Product data to create</param>
    /// <returns>The created product</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> CreateProduct(Product product)
    {
        logger.LogInformation("Creating new Product: {ProductName}", product.Name);

        if (string.IsNullOrEmpty(product.Name))
        {
            return BadRequest("Product name cannot be empty");
        }

        product.Id = products.Max(x => x.Id) + 1;
        product.CreatedAt = DateTime.UtcNow;
        
        products.Add(product);

        Response.Headers.Append("entity-id", product.Id.ToString());
        
        return Created();
    }

    /// <summary>
    /// Updates an existing product completely (replaces entire resource)
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="product">Updated product data</param>
    /// <returns>The updated product</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> UpdateProduct(int id, Product product)
    {
        logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        if (string.IsNullOrEmpty(product.Name))
        {
            return BadRequest("Product name cannot be empty");
        }

        var existingProduct = products.FirstOrDefault(p => p.Id == id);

        if (existingProduct == null)
        {
            return NotFound($"Product with ID {id} not fount for Update");
        }

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Category = product.Category;

        return Ok(existingProduct);
    }

    /// <summary>
    /// Partially updates an existing product (updates only provided fields)
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="product">Partial product data (only fields to update)</param>
    /// <returns>The updated product</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> PatchProduct(int id, Product product)
    {
        var existingProduct = products.FirstOrDefault(p => p.Id == id);

        if (existingProduct == null)
        {
            return NotFound($"Product with ID {id} not fount for Patch");
        }

        if (!string.IsNullOrEmpty(product.Name))
        {
            existingProduct.Name = product.Name;
        }
        
        if (product.Price > 0)
            existingProduct.Price = product.Price;

        if (!string.IsNullOrEmpty(product.Category))
            existingProduct.Category = product.Category;

        return Ok(existingProduct);
    }

    /// <summary>
    /// Deletes a product by ID
    /// </summary>
    /// <param name="id">Product ID to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete]
    public ActionResult DeleteProduct(int id)
    {
        var productToDelete = products.FirstOrDefault(x => x.Id == id);
        
        if (productToDelete == null)
        {
            return NotFound($"Product with ID {id} not fount for Delete");
        }

        products.Remove(productToDelete);

        return NoContent();
    }
}
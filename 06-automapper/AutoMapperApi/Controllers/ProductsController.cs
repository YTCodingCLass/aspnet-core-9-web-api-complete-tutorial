using AutoMapperApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoMapperApi.Controllers;

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
    public ActionResult<IEnumerable<ProductResponseDto>> GetProducts()
    {
        logger.LogInformation("Getting all Products");

        var products = ProductsController.products.Select(x => x.MapToResponseDto());

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
    public ActionResult<ProductResponseDto> GetProductById(int id)
    {
        var product = products.FirstOrDefault(x => x.Id == id);

        if (product == null)
            return NotFound();

        return Ok(product.MapToResponseDto());
    }
    
    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="createProductDto">Product data to create</param>
    /// <returns>The created product</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> CreateProduct(CreateProductDto createProductDto)
    {
        logger.LogInformation("Creating new Product: {ProductName}", createProductDto.Name);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newProduct = new Product()
        {
            Id = products.Max(x => x.Id) + 1,
            Name = createProductDto.Name,
            Price = createProductDto.Price,
            Category = createProductDto.Category,
            CreatedAt = DateTime.UtcNow
        };
        
        products.Add(newProduct);

        Response.Headers.Append("entity-id", newProduct.Id.ToString());
        
        return Created();
    }

    /// <summary>
    /// Updates an existing product completely (replaces entire resource)
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="updateProductDto">Updated product data</param>
    /// <returns>The updated product</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductResponseDto> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingProduct = products.FirstOrDefault(p => p.Id == id);

        if (existingProduct == null)
        {
            return NotFound($"Product with ID {id} not fount for Update");
        }

        existingProduct.Name = updateProductDto.Name;
        existingProduct.Price = updateProductDto.Price;
        existingProduct.Category = updateProductDto.Category;

        return Ok(existingProduct.MapToResponseDto());
    }

    /// <summary>
    /// Partially updates an existing product (updates only provided fields)
    /// </summary>
    /// <param name="id">Product ID to update</param>
    /// <param name="patchProductDto">Partial product data (only fields to update)</param>
    /// <returns>The updated product</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> PatchProduct(int id, PatchProductDto patchProductDto)
    {
        var existingProduct = products.FirstOrDefault(p => p.Id == id);

        if (existingProduct == null)
        {
            return NotFound($"Product with ID {id} not fount for Patch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!string.IsNullOrEmpty(patchProductDto.Name))
        {
            existingProduct.Name = patchProductDto.Name;
        }
        
        if (patchProductDto.Price > 0)
            existingProduct.Price = patchProductDto.Price.Value;

        if (!string.IsNullOrEmpty(patchProductDto.Category))
            existingProduct.Category = patchProductDto.Category;

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
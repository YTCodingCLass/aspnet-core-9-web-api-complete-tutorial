using AutoMapper;
using DependencyInjectionApi.Models;
using DependencyInjectionApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjectionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ILogger<ProductsController> logger, IMapper mapper) : ControllerBase
{
    // In-memory data store (in real app, this would be a database)
    private static readonly List<Product> products = new()
    {
        new Product 
        { 
            Id = 1, 
            Name = "Gaming Laptop", 
            Price = 1499.99m, 
            Category = "Electronics",
            StockQuantity = 25,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            Supplier = new Supplier 
            { 
                Id = 1, 
                CompanyName = "Tech Supplies Inc.", 
                ContactName = "John Doe",
                Phone = "555-0100"
            }
        },
        new Product 
        { 
            Id = 2, 
            Name = "Wireless Mouse", 
            Price = 49.99m, 
            Category = "Accessories",
            StockQuantity = 8, // Low stock!
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Supplier = new Supplier 
            { 
                Id = 2, 
                CompanyName = "Peripheral World", 
                ContactName = "Jane Smith",
                Phone = "555-0200"
            }
        },
        new Product 
        { 
            Id = 3, 
            Name = "Mechanical Keyboard", 
            Price = 129.99m, 
            Category = "Accessories",
            StockQuantity = 0, // Out of stock!
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            // No supplier for this one
        },
        new Product 
        { 
            Id = 4, 
            Name = "4K Monitor", 
            Price = 599.99m, 
            Category = "Displays",
            StockQuantity = 15,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        }
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
        
        var products = ProductsController.products;

        var productDtoList = mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        return Ok(productDtoList);
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

        var responseDto = mapper.Map<ProductResponseDto>(product);
        
        return Ok(responseDto);
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

        var newProduct = mapper.Map<Product>(createProductDto);
        newProduct.Id = products.Max(x => x.Id) + 1;
        
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

        mapper.Map(updateProductDto, existingProduct);

        var responseDto = mapper.Map<ProductResponseDto>(existingProduct);

        return Ok(responseDto);
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
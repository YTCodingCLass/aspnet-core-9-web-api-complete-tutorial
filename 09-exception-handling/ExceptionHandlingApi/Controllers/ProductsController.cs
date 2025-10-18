using Microsoft.AspNetCore.Mvc;
using ExceptionHandlingApi.Models.DTOs;
using ExceptionHandlingApi.Services;
using ExceptionHandlingApi.Exceptions;

namespace ExceptionHandlingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
    {
        var products = await productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProductsByCategory(string category)
    {
        // Example: Controller-level validation
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ValidationException("Category", "Category parameter is required");
        }

        if (category.Length < 2)
        {
            throw new ValidationException("Category", "Category must be at least 2 characters long");
        }

        var products = await productService.GetProductsByCategoryAsync(category);
        return Ok(products);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetLowStockProducts()
    {
        var products = await productService.GetLowStockProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
    {
        // Example: Parameter validation in controller
        if (id <= 0)
        {
            throw new ValidationException("Id", "Product ID must be greater than zero");
        }

        var product = await productService.GetProductByIdAsync(id);
        if (product == null)
        {
            throw new NotFoundException("Product", id);
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct(CreateProductDto createProductDto)
    {
        var newProduct = await productService.CreateProductAsync(createProductDto);
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        var updatedProduct = await productService.UpdateProductAsync(id, updateProductDto);
        if (updatedProduct == null)
        {
            throw new NotFoundException("Product", id);
        }
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await productService.DeleteProductAsync(id);
        if (!success)
        {
            throw new NotFoundException("Product", id);
        }
        return NoContent();
    }

    /// <summary>
    /// Example endpoint demonstrating multiple validation errors at once
    /// </summary>
    [HttpPost("bulk-create")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> BulkCreateProducts(List<CreateProductDto> products)
    {
        // Example: Multiple validation errors from controller
        var errors = new Dictionary<string, string[]>();

        if (products == null || !products.Any())
        {
            errors.Add("Products", ["At least one product must be provided"]);
        }
        else if (products.Count > 100)
        {
            errors.Add("Products", ["Cannot create more than 100 products at once"]);
        }

        // Check for duplicate names in the request
        if (products != null)
        {
            var duplicateNames = products
                .GroupBy(p => p.Name.ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Any())
            {
                errors.Add("Products", [$"Duplicate product names found: {string.Join(", ", duplicateNames)}"]);
            }
        }

        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        // Create products
        var createdProducts = new List<ProductResponseDto>();
        if (products == null)
            return Ok(createdProducts);
        foreach (var product in products)
        {
            var createdProduct = await productService.CreateProductAsync(product);
            createdProducts.Add(createdProduct);
        }

        return Ok(createdProducts);
    }
}

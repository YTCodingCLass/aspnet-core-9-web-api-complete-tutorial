using Microsoft.AspNetCore.Mvc;
using ConfigurationOptionsApi.Models.DTOs;
using ConfigurationOptionsApi.Services;

namespace ConfigurationOptionsApi.Controllers;

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
        var product = await productService.GetProductByIdAsync(id);
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
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await productService.DeleteProductAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Bulk create multiple products in a single request
    /// </summary>
    [HttpPost("bulk-create")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> BulkCreateProducts(List<CreateProductDto> products)
    {
        var createdProducts = await productService.BulkCreateProductsAsync(products);
        return Ok(createdProducts);
    }
}

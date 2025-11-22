# Chapter 05: DTOs & Validation - Professional API Design

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Validation](https://img.shields.io/badge/Validation-DataAnnotations-4CAF50?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Chapter 05: DTOs & Validation](https://www.youtube.com/watch?v=zwp3Qvxxzgc&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=5)**

## 🎯 Learning Objectives

By the end of this chapter, you'll master:
- ✅ Data Transfer Objects (DTOs) pattern
- ✅ Input validation with DataAnnotations
- ✅ Separating internal models from API contracts
- ✅ Custom validation messages and error handling
- ✅ Response mapping patterns

## 🚀 What We Build

A professional Product API with:
- **Separate DTOs** for requests and responses
- **Comprehensive validation** with custom error messages
- **Clean separation** between internal models and API contracts
- **Proper error responses** with validation details

## 📁 Project Structure

```
DtoAndValidation/
├── Controllers/
│   └── ProductsController.cs    # Updated controller with DTOs
├── Models/
│   ├── Product.cs              # Internal entity model
│   ├── CreateProductDto.cs     # DTO for creating products
│   ├── UpdateProductDto.cs     # DTO for updating products
│   ├── PatchProductDto.cs      # DTO for partial updates
│   └── ProductResponseDto.cs   # DTO for API responses
├── Program.cs                  # Validation configuration
└── DtoAndValidation.http       # HTTP requests with DTOs
```

## 🔧 DTO Pattern Implementation

### **Why DTOs Matter**
```csharp
// ❌ DON'T expose internal models directly
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string InternalNotes { get; set; }    // ⚠️ Internal only!
    public bool IsDeleted { get; set; }          // ⚠️ Internal only!
    public int CreatedByUserId { get; set; }     // ⚠️ Internal only!
    public decimal CostPrice { get; set; }       // ⚠️ Internal only!
}

// ✅ DO use DTOs for clean API contracts
public class CreateProductDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;
}
```

### **Request DTOs**

**CreateProductDto.cs**
```csharp
public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, 
        ErrorMessage = "Product name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, 
        ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, MinimumLength = 2, 
        ErrorMessage = "Category must be between 2 and 50 characters")]
    public string Category { get; set; } = string.Empty;
}
```

### **Response DTO**

**ProductResponseDto.cs**
```csharp
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    // Notice: No internal fields exposed!
}
```

## 🛡️ Validation in Action

### **DataAnnotations Attributes**
- `[Required]` - Field must be provided
- `[StringLength(max, MinimumLength = min)]` - String length constraints
- `[Range(min, max)]` - Numeric range validation
- `[EmailAddress]` - Email format validation
- `[RegularExpression]` - Custom pattern matching

### **Controller with DTO Mapping**
```csharp
[HttpPost]
public ActionResult<ProductResponseDto> CreateProduct(CreateProductDto createDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var product = new Product
    {
        Name = createDto.Name,
        Price = createDto.Price,
        Category = createDto.Category,
        Id = products.Max(x => x.Id) + 1,
        CreatedAt = DateTime.UtcNow
    };

    products.Add(product);
    return Ok(product.MapToResponseDto());
}
```

## 🧪 Testing Validation

### **Valid Request**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Gaming Keyboard",
  "price": 129.99,
  "category": "Gaming"
}
```

### **Invalid Request (Triggers Validation)**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "",           // ❌ Required field empty
  "price": -10,         // ❌ Negative price
  "category": "A"       // ❌ Too short (min 2 chars)
}
```

### **Validation Error Response**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Product name is required"],
    "Price": ["Price must be greater than 0"],
    "Category": ["Category must be between 2 and 50 characters"]
  }
}
```

## 🎭 DTO Mapping Patterns

### **Manual Mapping (Current Implementation)**
```csharp
// In Product.cs
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
```

### **Benefits of This Pattern**
1. **Security** - Internal fields never exposed
2. **Flexibility** - API can evolve independently from database
3. **Validation** - Strong input validation at API boundary
4. **Documentation** - Clear contracts for API consumers

## 🔍 DTO Types Explained

### **CreateProductDto** 
- Used for **POST** requests
- Contains only fields needed for creation
- Heavy validation for data integrity

### **UpdateProductDto**
- Used for **PUT** requests (complete updates)
- Contains all updatable fields
- Validation ensures complete resource replacement

### **PatchProductDto**
- Used for **PATCH** requests (partial updates)
- All fields optional for flexibility
- Conditional validation based on provided fields

### **ProductResponseDto**
- Used for all **response** operations
- Contains only public-facing fields
- Consistent response format across endpoints

## 📊 Before vs After Comparison

### **Before DTOs (Chapter 04)**
```csharp
// ❌ Exposing internal model
[HttpPost]
public ActionResult<Product> CreateProduct(Product product)
{
    // Internal fields could be set by clients!
    // No validation boundaries
}
```

### **After DTOs (Chapter 05)**
```csharp
// ✅ Clean DTO-based approach
[HttpPost]
public ActionResult<ProductResponseDto> CreateProduct(CreateProductDto createDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
        
    // Map DTO to internal model
    // Only expose public fields in response
}
```

## 🧪 Complete API Testing

### **Create Product with Validation**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Wireless Gaming Mouse",
  "price": 89.99,
  "category": "Gaming Accessories"
}
```

### **Update Product (PUT)**
```http
PUT https://localhost:7185/api/products/1
Content-Type: application/json

{
  "name": "Updated Gaming Mouse",
  "price": 99.99,
  "category": "Gaming"
}
```

### **Partial Update (PATCH)**
```http
PATCH https://localhost:7185/api/products/1
Content-Type: application/json

{
  "price": 79.99
}
```

## 🔧 Running the Project

```bash
cd 05-dto-and-validations/DtoAndValidation
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`

## 🎓 Best Practices Demonstrated

1. **Input Validation** - Never trust client input
2. **Response Consistency** - Same format across all endpoints
3. **Security** - Internal data never exposed
4. **Maintainability** - Easy to change internal models
5. **Documentation** - Clear API contracts in Swagger

## 🚨 Common Validation Scenarios

### **String Validation**
```csharp
[Required(ErrorMessage = "Name is required")]
[StringLength(100, MinimumLength = 2)]
[RegularExpression(@"^[a-zA-Z0-9\s]+$", 
    ErrorMessage = "Name contains invalid characters")]
```

### **Numeric Validation**
```csharp
[Required]
[Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
```

### **Custom Validation**
```csharp
[Required]
[RegularExpression(@"^(Electronics|Gaming|Accessories|Software)$", 
    ErrorMessage = "Category must be: Electronics, Gaming, Accessories, or Software")]
```

## ➡️ Next Steps

Ready to eliminate manual mapping with AutoMapper?
**[Chapter 06: AutoMapper Integration](../06-automapper/)**

## 🤔 Troubleshooting

**Validation not working?**
- Ensure `[ApiController]` attribute is present
- Check that ModelState.IsValid is being checked

**Missing validation errors in response?**
- Verify you're returning `BadRequest(ModelState)`
- Check that error messages are properly set

---

**💡 Pro Tip**: Always validate at the API boundary - never trust incoming data, even from trusted clients!
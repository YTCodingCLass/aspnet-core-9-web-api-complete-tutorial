# Chapter 04: HTTP Methods API - Complete CRUD Operations

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![REST](https://img.shields.io/badge/REST-API-009688?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Chapter 04: HTTP Methods API](https://www.youtube.com/watch?v=3pkXQIpd-Tk&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=6)**

## 🎯 Learning Objectives

By the end of this chapter, you'll master:
- ✅ All HTTP methods: GET, POST, PUT, PATCH, DELETE
- ✅ RESTful API design principles
- ✅ Proper HTTP status codes usage
- ✅ Request/response handling patterns
- ✅ Error handling and logging

## 🚀 What We Build

A complete Product Management API with full CRUD operations:
- **C**reate products (POST)
- **R**ead products (GET)
- **U**pdate products (PUT & PATCH)
- **D**elete products (DELETE)

## 📁 Project Structure

```
HttpMethodsApi/
├── Controllers/
│   └── ProductsController.cs    # Complete CRUD controller
├── Models/
│   └── Product.cs              # Product entity model
├── Program.cs                  # Swagger + logging config
├── HttpMethodsApi.http         # All HTTP method examples
└── [Standard ASP.NET Core files]
```

## 🛠️ API Endpoints

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| **GET** | `/api/products` | Get all products | 200 OK |
| **GET** | `/api/products/{id}` | Get product by ID | 200 OK, 404 Not Found |
| **POST** | `/api/products` | Create new product | 201 Created, 400 Bad Request |
| **PUT** | `/api/products/{id}` | Update entire product | 200 OK, 400 Bad Request, 404 Not Found |
| **PATCH** | `/api/products/{id}` | Partial update | 200 OK, 400 Bad Request, 404 Not Found |
| **DELETE** | `/api/products/{id}` | Delete product | 204 No Content, 404 Not Found |

## 💻 Code Examples

### **GET - Retrieve All Products**
```csharp
[HttpGet]
[ProducesResponseType(StatusCodes.Status200OK)]
public ActionResult<IEnumerable<Product>> GetProduct()
{
    logger.LogInformation("Getting all Products");
    return Ok(products);
}
```

### **POST - Create New Product**
```csharp
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public ActionResult<Product> CreateProduct(Product product)
{
    if (string.IsNullOrEmpty(product.Name))
        return BadRequest("Product name cannot be empty");

    product.Id = products.Max(x => x.Id) + 1;
    product.CreatedAt = DateTime.UtcNow;
    products.Add(product);

    Response.Headers.Append("entity-id", product.Id.ToString());
    return Created();
}
```

### **PUT vs PATCH - Understanding the Difference**

**PUT (Complete Replacement)**
```csharp
[HttpPut("{id}")]
public ActionResult<Product> UpdateProduct(int id, Product product)
{
    // Replaces ALL properties of the existing product
    existingProduct.Name = product.Name;
    existingProduct.Price = product.Price;
    existingProduct.Category = product.Category;
}
```

**PATCH (Partial Update)**
```csharp
[HttpPatch("{id}")]
public ActionResult<Product> PatchProduct(int id, Product product)
{
    // Only updates NON-NULL/NON-DEFAULT properties
    if (!string.IsNullOrEmpty(product.Name))
        existingProduct.Name = product.Name;
    
    if (product.Price > 0)
        existingProduct.Price = product.Price;
}
```

## 🔍 HTTP Methods Deep Dive

### **GET - Safe & Idempotent**
- **Purpose**: Retrieve data without side effects
- **Characteristics**: Safe, idempotent, cacheable
- **Use Cases**: Getting single items or collections

### **POST - Neither Safe nor Idempotent**
- **Purpose**: Create new resources
- **Characteristics**: Not safe, not idempotent
- **Response**: 201 Created with Location header

### **PUT - Idempotent but Not Safe**
- **Purpose**: Complete resource replacement
- **Characteristics**: Idempotent, not safe
- **Rule**: Send ALL required fields

### **PATCH - Neither Safe nor Idempotent**
- **Purpose**: Partial resource updates
- **Characteristics**: Not safe, not idempotent
- **Rule**: Send only fields to update

### **DELETE - Idempotent but Not Safe**
- **Purpose**: Remove resources
- **Characteristics**: Idempotent, not safe
- **Response**: 204 No Content on success

## 🧪 Testing the API

### **Using the HTTP File**
Open `HttpMethodsApi.http` and run these requests:

```http
### Get all products
GET https://localhost:7185/api/products

### Get product by ID
GET https://localhost:7185/api/products/1

### Create new product
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Gaming Mouse",
  "price": 59.99,
  "category": "Gaming"
}

### Update entire product
PUT https://localhost:7185/api/products/1
Content-Type: application/json

{
  "name": "Updated Laptop",
  "price": 1299.99,
  "category": "Electronics"
}

### Partial update
PATCH https://localhost:7185/api/products/1
Content-Type: application/json

{
  "price": 899.99
}

### Delete product
DELETE https://localhost:7185/api/products/1
```

## 📊 Response Examples

### **Successful GET Response (200 OK)**
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "price": 999.99,
    "category": "Electronics",
    "createdAt": "2025-07-25T10:30:00Z"
  }
]
```

### **Create Response (201 Created)**
```
Status: 201 Created
Headers:
  entity-id: 5
  location: /api/products/5
```

### **Error Response (404 Not Found)**
```
Status: 404 Not Found
```

## 🎓 Best Practices Demonstrated

1. **Proper HTTP Status Codes** - Each method returns appropriate codes
2. **Logging Integration** - Track API usage and debugging
3. **Input Validation** - Basic validation before processing
4. **Response Headers** - Custom headers for created resources
5. **XML Documentation** - Professional API documentation

## 🔧 Running the Project

```bash
cd 04-http-methods-api/HttpMethodsApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`

## ➡️ Next Steps

Want to make your API more professional with proper validation and DTOs?
**[Chapter 05: DTOs & Validation](../05-dto-and-validations/)**

## 🤔 Troubleshooting

**400 Bad Request on POST?**
- Check JSON format and required fields
- Ensure Content-Type header is set to `application/json`

**404 Not Found?**
- Verify the product ID exists
- Check the URL path matches the route pattern

---

**💡 Pro Tip**: Use PATCH for partial updates and PUT only when you want to replace the entire resource!
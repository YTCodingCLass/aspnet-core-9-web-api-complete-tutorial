# Custom Middleware in ASP.NET Core

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Middleware](https://img.shields.io/badge/Custom-Middleware-FF6B35?style=flat-square)
![Pipeline](https://img.shields.io/badge/Request-Pipeline-2E8B57?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Custom Middleware Tutorial](#)** *(Add your video link here)*

## 🎯 Learning Objectives

By the end of this tutorial, you'll master:
- ✅ **Custom Middleware** - Creating custom middleware components
- ✅ **Request Pipeline** - Understanding the ASP.NET Core request pipeline
- ✅ **Middleware Ordering** - Proper middleware registration and execution order
- ✅ **Request/Response Manipulation** - Modifying HTTP requests and responses
- ✅ **Short-Circuiting** - Terminating the pipeline early when needed
- ✅ **Production-Ready Middleware** - Environment-aware middleware behavior

## 🚀 What We Build

A **Production-Ready Custom Middleware System** that demonstrates:
- **Request Logging Middleware** - Logs incoming HTTP requests
- **Response Timing Middleware** - Measures request processing time
- **Custom Header Middleware** - Adds custom headers to responses
- **API Key Validation Middleware** - Simple authentication middleware
- **Exception Handling Middleware** - Centralized error handling
- **Request/Response Body Reading** - Reading and logging request/response bodies

## 📁 Project Structure

```
CustomMiddlewareApi/
├── Controllers/
│   └── ProductsController.cs        # API endpoints
├── Middleware/                       # ⭐ Custom middleware components
│   ├── RequestLoggingMiddleware.cs  # Logs HTTP requests
│   ├── ResponseTimingMiddleware.cs  # Measures response time
│   ├── CustomHeaderMiddleware.cs    # Adds custom headers
│   └── ApiKeyMiddleware.cs          # API key validation
├── Exceptions/                       # Custom exception types
│   ├── BaseException.cs             # Base exception with status code
│   ├── NotFoundException.cs         # 404 Not Found
│   ├── BadRequestException.cs       # 400 Bad Request
│   ├── ValidationException.cs       # 422 Unprocessable Entity
│   ├── UnauthorizedException.cs     # 401 Unauthorized
│   ├── ForbiddenException.cs        # 403 Forbidden
│   └── ConflictException.cs         # 409 Conflict
├── Handlers/                         # Exception handlers
│   ├── GlobalExceptionHandler.cs    # Catches all unhandled exceptions
│   ├── BusinessExceptionHandler.cs  # Handles business exceptions
│   └── ValidationExceptionHandler.cs # Handles validation errors
├── Models/
│   ├── Product.cs                   # Product entity
│   └── DTOs/
│       └── ProductDtos.cs           # Product DTOs
├── Repositories/
│   ├── IProductRepository.cs        # Repository interface
│   └── ProductRepository.cs         # Repository implementation
├── Services/
│   ├── IProductService.cs           # Service interface
│   └── ProductService.cs            # Service with validation logic
├── Data/
│   └── InMemoryDatabase.cs          # In-memory data store
├── Mappings/
│   └── MappingProfile.cs            # AutoMapper configuration
├── Program.cs                       # Middleware pipeline configuration ⭐
└── CustomMiddlewareApi.http         # HTTP requests for testing
```

## 🏗️ Middleware Pipeline Architecture

### **Request Pipeline Flow**

```
┌─────────────────────────────────────────────────────┐
│              Incoming HTTP Request                  │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      1. Exception Handler Middleware                │
│         • Wraps entire pipeline                     │
│         • Catches unhandled exceptions              │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      2. Request Logging Middleware                  │
│         • Logs request method, path, query          │
│         • Records request timestamp                 │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      3. Response Timing Middleware                  │
│         • Starts timer                              │
│         • Adds X-Response-Time header               │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      4. Custom Header Middleware                    │
│         • Adds X-Custom-Header                      │
│         • Adds X-Server-Info                        │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      5. API Key Middleware (Optional)               │
│         • Validates X-API-Key header                │
│         • Returns 401 if invalid/missing            │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│            Built-in Middleware                      │
│         • Routing                                   │
│         • CORS                                      │
│         • Authentication                            │
│         • Authorization                             │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              Controllers/Endpoints                  │
│         • Process request                           │
│         • Call services                             │
│         • Return response                           │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
      Response bubbles back through middleware
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              HTTP Response to Client                │
│         • Status code                               │
│         • Headers (including custom)                │
│         • Body                                      │
└─────────────────────────────────────────────────────┘
```

## 🎨 Custom Exception Hierarchy

### **Base Exception**
```csharp
// Exceptions/BaseException.cs
public abstract class BaseException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected BaseException(string message, int statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
```

### **Specific Exceptions**

| Exception | HTTP Status | Error Code | Use Case |
|-----------|-------------|------------|----------|
| `NotFoundException` | 404 | `NOT_FOUND` | Resource not found |
| `BadRequestException` | 400 | `BAD_REQUEST` | Invalid request data or business rule violation |
| `ValidationException` | 422 | `VALIDATION_ERROR` | Input validation failures (with field-level errors) |
| `UnauthorizedException` | 401 | `UNAUTHORIZED` | Authentication required |
| `ForbiddenException` | 403 | `FORBIDDEN` | Insufficient permissions |
| `ConflictException` | 409 | `CONFLICT` | Resource conflict (e.g., duplicate name) |

### **Exception Examples**

```csharp
// Single field validation
throw new ValidationException("Id", "Product ID must be greater than zero");

// Multiple field validation
var errors = new Dictionary<string, string[]>
{
    { "Name", new[] { "Product name is required" } },
    { "Price", new[] { "Price must be greater than zero" } }
};
throw new ValidationException(errors);

// Not found
throw new NotFoundException("Product", id);

// Business rule violation
throw new BadRequestException("Price must be at least 10% higher than cost price");

// Conflict
throw new ConflictException($"Product with name '{name}' already exists");
```

## 🔧 Exception Handlers Implementation

### **1. Validation Exception Handler** (Most Specific)
```csharp
// Handlers/ValidationExceptionHandler.cs
public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false; // Let next handler try
        }

        logger.LogWarning("Validation error: {ErrorCount} errors",
            validationException.Errors.Count);

        var problemDetails = new ValidationProblemDetails(validationException.Errors)
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = validationException.Message,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["errorCode"] = "VALIDATION_ERROR",
                ["timestamp"] = DateTimeOffset.UtcNow,
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
```

### **2. Business Exception Handler** (Specific)
```csharp
// Handlers/BusinessExceptionHandler.cs
public class BusinessExceptionHandler(ILogger<BusinessExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BaseException baseException ||
            exception is ValidationException)
        {
            return false; // ValidationException handled by specific handler
        }

        logger.LogWarning("Business exception: {ErrorCode} - {Message}",
            baseException.ErrorCode, baseException.Message);

        var problemDetails = new ProblemDetails
        {
            Type = GetProblemTypeUrl(baseException.StatusCode),
            Title = GetStatusTitle(baseException.StatusCode),
            Status = baseException.StatusCode,
            Detail = baseException.Message,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["errorCode"] = baseException.ErrorCode,
                ["timestamp"] = DateTimeOffset.UtcNow,
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };

        httpContext.Response.StatusCode = baseException.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
```

### **3. Global Exception Handler** (Catch-All)
```csharp
// Handlers/GlobalExceptionHandler.cs
public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = environment.IsDevelopment()
                ? exception.Message
                : "An error occurred while processing your request.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["errorCode"] = "INTERNAL_SERVER_ERROR",
                ["timestamp"] = DateTimeOffset.UtcNow,
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };

        // In development, add debug information
        if (environment.IsDevelopment())
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    message = exception.InnerException.Message,
                    type = exception.InnerException.GetType().Name
                };
            }
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
```

## ⚙️ Service Layer Validation

### **Moving Validation from Controllers to Services**

**❌ Before (Controller-based validation):**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
{
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
```

**✅ After (Service-based validation):**
```csharp
// Controller - Thin and clean
[HttpGet("{id}")]
public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
{
    var product = await productService.GetProductByIdAsync(id);
    return Ok(product);
}

// Service - Handles all validation
public async Task<ProductResponseDto> GetProductByIdAsync(int id)
{
    // Validation
    if (id <= 0)
    {
        throw new ValidationException("Id", "Product ID must be greater than zero");
    }

    var product = await productRepository.GetByIdWithSupplierAsync(id);
    if (product == null)
    {
        throw new NotFoundException("Product", id);
    }

    var dto = mapper.Map<ProductResponseDto>(product);
    dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
    return dto;
}
```

### **Comprehensive Validation Example**

```csharp
private async Task ValidateProductCreationAsync(CreateProductDto createDto)
{
    var errors = new Dictionary<string, string[]>();

    // Validation: Product name
    if (string.IsNullOrWhiteSpace(createDto.Name))
    {
        errors.Add("Name", ["Product name is required"]);
    }
    else if (createDto.Name.Length < 3)
    {
        errors.Add("Name", ["Product name must be at least 3 characters long"]);
    }
    else if (createDto.Name.Length > 100)
    {
        errors.Add("Name", ["Product name cannot exceed 100 characters"]);
    }

    // Validation: Price
    if (createDto.Price <= 0)
    {
        errors.Add("Price", ["Price must be greater than zero"]);
    }
    else if (createDto.Price > 1000000)
    {
        errors.Add("Price", ["Price cannot exceed 1,000,000"]);
    }

    // Validation: Stock Quantity
    if (createDto.StockQuantity < 0)
    {
        errors.Add("StockQuantity", ["Stock quantity cannot be negative"]);
    }

    // Throw ValidationException if there are any errors
    if (errors.Any())
    {
        throw new ValidationException(errors);
    }

    // Business rule: Check if product name already exists
    var existingProducts = await productRepository.GetAllAsync();
    if (existingProducts.Any(p => p.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
    {
        throw new ConflictException($"Product with name '{createDto.Name}' already exists");
    }

    // Business rule: Validate profit margin
    if (createDto.Price <= createDto.CostPrice * 1.1m)
    {
        throw new BadRequestException("Price must be at least 10% higher than cost price");
    }
}
```

## 🔌 Program.cs Configuration

```csharp
// Register Problem Details service
builder.Services.AddProblemDetails();

// Register exception handlers
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Register services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// *** MIDDLEWARE PIPELINE - ORDER MATTERS! ***

// 1. Exception handler wraps everything
app.UseExceptionHandler();

// 2. Custom middleware components
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ResponseTimingMiddleware>();
app.UseMiddleware<CustomHeaderMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>(); // Optional authentication

// 3. Built-in middleware
app.UseSwagger();
app.UseHttpsRedirection();
app.UseAuthorization();

// 4. Endpoint routing
app.MapControllers();

app.Run();
```

## 📋 RFC 7807 Problem Details Response Format

### **Validation Error Response (422)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 422,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/products",
  "errors": {
    "Name": [
      "Product name is required"
    ],
    "Price": [
      "Price must be greater than zero"
    ],
    "Category": [
      "Category is required"
    ]
  },
  "errorCode": "VALIDATION_ERROR",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **Not Found Error Response (404)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Product with key '999' was not found.",
  "instance": "/api/products/999",
  "errorCode": "NOT_FOUND",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **Business Rule Violation (400)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Price must be at least 10% higher than cost price",
  "instance": "/api/products",
  "errorCode": "BAD_REQUEST",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **Conflict Error (409)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Product with name 'Laptop Pro' already exists",
  "instance": "/api/products",
  "errorCode": "CONFLICT",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **Unhandled Exception - Development (500)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Object reference not set to an instance of an object.",
  "instance": "/api/products",
  "errorCode": "INTERNAL_SERVER_ERROR",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00",
  "exceptionType": "NullReferenceException",
  "stackTrace": "at ExceptionHandlingApi.Services.ProductService.GetAllProductsAsync()..."
}
```

### **Unhandled Exception - Production (500)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An error occurred while processing your request.",
  "instance": "/api/products",
  "errorCode": "INTERNAL_SERVER_ERROR",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

## 🧪 Testing the Exception Handling

### **Test Scenarios**

1. **Validation Error** - Empty product name
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "",
  "price": -10,
  "category": ""
}
```

2. **Not Found** - Invalid product ID
```http
GET https://localhost:7xxx/api/products/99999
```

3. **Business Rule Violation** - Low profit margin
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Test Product",
  "price": 100,
  "costPrice": 95,
  "category": "Test"
}
```

4. **Conflict** - Duplicate product name
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Laptop Pro",
  "price": 1200,
  "costPrice": 800,
  "category": "Electronics"
}
```

5. **Bulk Create with Validation**
```http
POST https://localhost:7xxx/api/products/bulk-create
Content-Type: application/json

[
  {
    "name": "Product 1",
    "price": 100
  },
  {
    "name": "Product 1",
    "price": 100
  }
]
```

## 🎓 Key Benefits

### **1. Centralized Error Handling**
- ✅ All exceptions handled in one place
- ✅ No try-catch blocks in controllers
- ✅ Consistent error response format
- ✅ Easy to maintain and extend

### **2. RFC 7807 Compliance**
- ✅ Standard problem details format
- ✅ Machine-readable error responses
- ✅ Better client integration
- ✅ Industry best practices

### **3. Clean Controllers**
- ✅ Thin controllers with no exception handling logic
- ✅ Focus on HTTP concerns only
- ✅ Better readability and maintainability

### **4. Service Layer Validation**
- ✅ Reusable validation logic
- ✅ Works across different entry points (REST, gRPC, etc.)
- ✅ Better separation of concerns
- ✅ Easier to test

### **5. Environment-Aware**
- ✅ Detailed error info in Development
- ✅ Sanitized responses in Production
- ✅ Security best practices

### **6. Structured Exception Hierarchy**
- ✅ Clear exception types for different scenarios
- ✅ HTTP status codes built-in
- ✅ Custom error codes for client handling
- ✅ Type-safe exception handling

## 🔧 Running the Project

```bash
cd CustomMiddlewareApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`

## 🎯 Key Takeaways

1. **Custom Middleware**: Create reusable components for cross-cutting concerns
2. **Pipeline Order**: Middleware registration order is critical - think carefully about the sequence
3. **Request Delegate**: Use `next()` to pass control to the next middleware
4. **Short-Circuiting**: Return early without calling `next()` when needed
5. **Request/Response Access**: Full access to HttpContext for reading/modifying requests and responses
6. **Exception Handling**: Place exception middleware early to catch errors from downstream middleware
7. **Environment Awareness**: Enable/disable middleware based on environment

## ➡️ What's Next?

**Extend this middleware system with:**
- **Rate Limiting Middleware** - Limit requests per IP/user
- **Caching Middleware** - Response caching for improved performance
- **Compression Middleware** - Compress responses to reduce bandwidth
- **CORS Middleware** - Handle cross-origin requests
- **Request/Response Logging** - Log full request/response bodies for debugging
- **Performance Monitoring** - Track slow endpoints and bottlenecks

---

**💡 Pro Tip**: Middleware order matters! Exception handlers should be first, followed by logging, then authentication/authorization, and routing comes last before endpoints!

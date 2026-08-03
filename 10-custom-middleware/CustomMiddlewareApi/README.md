# Custom Middleware in ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Middleware](https://img.shields.io/badge/Custom-Middleware-FF6B35?style=flat-square)
![Pipeline](https://img.shields.io/badge/Request-Pipeline-2E8B57?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Custom Middleware Tutorial](https://www.youtube.com/watch?v=n1A_IjEf_hs&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=3)**

## 🎯 Learning Objectives

By the end of this tutorial, you'll master:
- ✅ **Custom Middleware** - Creating custom middleware using `IMiddleware` interface
- ✅ **Request Pipeline** - Understanding the ASP.NET Core middleware pipeline
- ✅ **Middleware Ordering** - Critical importance of middleware registration order
- ✅ **Request/Response Logging** - Reading and logging HTTP request/response data
- ✅ **Performance Monitoring** - Measuring request processing time with stopwatch
- ✅ **Response Headers** - Adding custom headers to HTTP responses
- ✅ **Production-Ready Patterns** - Building reusable middleware components

## 🚀 What We Build

A **Production-Ready Custom Middleware System** featuring three specialized middleware components:

1. **RequestLoggingMiddleware** - Logs incoming request details and outgoing response status
2. **ResponseTimingMiddleware** - Measures request processing time and adds `X-Response-Time` header
3. **RequestResponseLoggingMiddleware** - Detailed request/response body logging for debugging

## 📁 Project Structure

```
CustomMiddlewareApi/
├── Controllers/
│   └── ProductsController.cs        # API endpoints
├── Middleware/                       # ⭐ Custom middleware components
│   ├── RequestLoggingMiddleware.cs  # Logs HTTP requests and responses
│   ├── ResponseTimingMiddleware.cs  # Measures and logs response time
│   └── RequestResponseLoggingMiddleware.cs # Detailed body logging
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
│   ├── Supplier.cs                  # Supplier entity
│   └── DTOs/
│       ├── CreateProductDto.cs      # Product creation request
│       ├── UpdateProductDto.cs      # Product update request
│       └── ProductResponseDto.cs    # Product response with stock status
├── Repositories/
│   ├── IProductRepository.cs        # Repository interface
│   └── ProductRepository.cs         # Repository implementation
├── Services/
│   ├── IProductService.cs           # Service interface
│   ├── ProductService.cs            # Service with validation logic
│   ├── INotificationService.cs      # Notification service interface
│   └── NotificationService.cs       # Notification implementation
├── Data/
│   └── ProductsData.cs              # In-memory data store
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
│         • Returns RFC 7807 Problem Details          │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      2. Request/Response Logging Middleware         │
│         • Reads and logs request body               │
│         • Enables request buffering                 │
│         • Captures response body                    │
│         • Logs complete request/response details    │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      3. Response Timing Middleware                  │
│         • Starts stopwatch                          │
│         • Adds X-Response-Time header               │
│         • Logs processing duration                  │
│         • Warns if request > 1000ms                 │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      4. Request Logging Middleware                  │
│         • Logs request method, path                 │
│         • Logs remote IP address                    │
│         • Logs response status code                 │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│            Built-in Middleware                      │
│         • Swagger (Development only)                │
│         • HTTPS Redirection                         │
│         • Authorization                             │
│         • Endpoint Routing                          │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              Controllers/Endpoints                  │
│         • ProductsController                        │
│         • Call services                             │
│         • Return responses                          │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
      Response bubbles back through middleware
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              HTTP Response to Client                │
│         • Status code                               │
│         • Headers (including X-Response-Time)       │
│         • JSON body                                 │
└─────────────────────────────────────────────────────┘
```

## 💻 Middleware Implementation Details

### **1. RequestLoggingMiddleware** - Basic Request/Response Logging

```csharp
public class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Before the request
        logger.LogInformation(
            "Incoming Request: {Method} {Path} from {RemoteIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        // Call the next middleware in the pipeline
        await next(context);

        // After the response
        logger.LogInformation(
            "Outgoing Response: {StatusCode} for {Method} {Path}",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path);
    }
}
```

**Key Concepts:**
- Implements `IMiddleware` interface for dependency injection support
- Uses **primary constructor** (C# 12 feature) for cleaner code
- Logs **before** calling `next()` for incoming request
- Logs **after** calling `next()` for outgoing response
- Captures **RemoteIpAddress** for tracking client requests

---

### **2. ResponseTimingMiddleware** - Performance Monitoring

```csharp
public class ResponseTimingMiddleware(ILogger<ResponseTimingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Start the stopwatch
        var stopwatch = Stopwatch.StartNew();

        // Hook into OnStarting to add the header before the response is sent
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            context.Response.Headers.Append("X-Response-Time", $"{elapsedMilliseconds}ms");
            return Task.CompletedTask;
        });

        // Execute the next middleware
        await next(context);

        // Stop the stopwatch (if not already stopped)
        stopwatch.Stop();

        // Log the elapsed time
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        logger.LogInformation(
            "Request {Method} {Path} completed in {ElapsedMilliseconds}ms with status {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            elapsedMilliseconds,
            context.Response.StatusCode);

        // Warn if request took too long
        if (elapsedMilliseconds > 1000)
        {
            logger.LogWarning(
                "SLOW REQUEST: {Method} {Path} took {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds);
        }
    }
}
```

**Key Concepts:**
- Uses `Stopwatch` for accurate time measurement
- **OnStarting callback** - Ensures header is added before response starts
- Adds **X-Response-Time** custom header to every response
- Logs performance metrics with structured logging
- **Slow request detection** - Warns if response time > 1000ms
- Ideal for identifying performance bottlenecks

---

### **3. RequestResponseLoggingMiddleware** - Detailed Body Logging

```csharp
public class RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Log Request
        await LogRequest(context);

        // Copy the original response stream
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        // Execute the next middleware
        await next(context);

        // Log Response
        await LogResponse(context);

        // Copy the contents of the new response stream to the original stream
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        logger.LogInformation(
            "HTTP Request Information:\n" +
            "Method: {Method}\n" +
            "Path: {Path}\n" +
            "QueryString: {QueryString}\n" +
            "Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            body);
    }

    private async Task LogResponse(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        logger.LogInformation(
            "HTTP Response Information:\n" +
            "StatusCode: {StatusCode}\n" +
            "Body: {Body}",
            context.Response.StatusCode,
            body);
    }
}
```

**Key Concepts:**
- **EnableBuffering()** - Allows reading request body multiple times
- **Stream replacement** - Captures response body without breaking the response
- Logs complete request body (method, path, query, body)
- Logs complete response body (status code, body)
- **Important**: Reset stream position after reading to avoid data loss
- **Use cautiously** - Can be verbose in production, enable only for debugging

---

## 🔌 Program.cs Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Register exception handlers
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register services for dependency injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

// ⭐ Register middleware services (required for IMiddleware interface)
builder.Services.AddScoped<RequestResponseLoggingMiddleware>();
builder.Services.AddScoped<ResponseTimingMiddleware>();
builder.Services.AddScoped<RequestLoggingMiddleware>();

var app = builder.Build();

// *** MIDDLEWARE ORDER MATTERS! ***

// 1. Exception Handler - wraps the entire pipeline to catch all exceptions
app.UseExceptionHandler();

// 2. Custom Middleware - Request/Response Logging (optional - can be verbose)
// Uncomment to enable detailed request/response body logging
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// 3. Custom Middleware - Response Timing
// Measures how long each request takes and adds X-Response-Time header
app.UseMiddleware<ResponseTimingMiddleware>();

// 4. Custom Middleware - Request Logging
// Logs basic request information (method, path, IP)
app.UseMiddleware<RequestLoggingMiddleware>();

// 5. Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. HTTPS Redirection
app.UseHttpsRedirection();

// 7. Authorization
app.UseAuthorization();

// 8. Endpoint Routing - maps controllers
app.MapControllers();

app.Run();
```

**Critical Points:**
- **IMiddleware registration**: Must register middleware classes in DI container
- **Middleware order**: Exception handler first, then custom middleware, then built-in
- **Scoped lifetime**: Middleware using `IMiddleware` should be scoped
- **Environment-aware**: Swagger only in Development

---

## 🧪 Testing the Middleware

### **1. Test Response Timing**
```http
GET https://localhost:7xxx/api/products
```

**Check response headers** for:
```
X-Response-Time: 25ms
```

**Check logs** for:
```
Request GET /api/products completed in 25ms with status 200
```

---

### **2. Test Request Logging**
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Test Product",
  "price": 99.99,
  "stockQuantity": 10
}
```

**Check logs** for:
```
Incoming Request: POST /api/products from ::1
Outgoing Response: 201 for POST /api/products
```

---

### **3. Test Detailed Body Logging**

Enable `RequestResponseLoggingMiddleware` in Program.cs and test:

```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Laptop Pro",
  "price": 1299.99,
  "stockQuantity": 5
}
```

**Check logs** for complete request/response bodies:
```
HTTP Request Information:
Method: POST
Path: /api/products
QueryString:
Body: {"name":"Laptop Pro","price":1299.99,"stockQuantity":5}

HTTP Response Information:
StatusCode: 201
Body: {"id":4,"name":"Laptop Pro","price":1299.99,...}
```

---

### **4. Test Slow Request Warning**

Trigger a slow endpoint and check for warning:

```
SLOW REQUEST: GET /api/products/slow-endpoint took 1523ms
```

---

## 🎓 Key Benefits

### **1. Separation of Concerns**
- ✅ Middleware handles cross-cutting concerns (logging, timing, headers)
- ✅ Controllers focus on business logic
- ✅ Clean, maintainable codebase

### **2. Reusability**
- ✅ Middleware components work across all endpoints
- ✅ No code duplication in controllers
- ✅ Easy to enable/disable features

### **3. Performance Monitoring**
- ✅ Automatic response time tracking
- ✅ Slow request detection
- ✅ Performance bottleneck identification

### **4. Debugging**
- ✅ Complete request/response logging
- ✅ IP address tracking
- ✅ Structured logging with correlation

### **5. IMiddleware Interface Benefits**
- ✅ Dependency injection support
- ✅ Scoped services (logger, DbContext, etc.)
- ✅ Cleaner, testable code
- ✅ Lifetime management by DI container

---

## 🔧 Running the Project

```bash
cd 10-custom-middleware/CustomMiddlewareApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`

---

## 🎯 Key Takeaways

1. **IMiddleware Interface**: Preferred approach for middleware with DI support
2. **Primary Constructors**: Use C# 12 feature for cleaner constructor injection
3. **Middleware Order**: Critical - Exception handler first, routing last
4. **Performance Monitoring**: Use Stopwatch and OnStarting callback for headers
5. **Request Buffering**: EnableBuffering() allows reading request body multiple times
6. **Stream Management**: Replace response stream to capture body without breaking response
7. **Environment Awareness**: Enable verbose logging only in Development
8. **Structured Logging**: Use named placeholders for better log searching

---

## ➡️ What's Next?

**Extend this middleware system with:**
- **Authentication Middleware** - API key or JWT validation
- **Rate Limiting Middleware** - Limit requests per IP/user
- **Caching Middleware** - Response caching for improved performance
- **Compression Middleware** - Compress responses to reduce bandwidth
- **CORS Middleware** - Handle cross-origin requests
- **Short-Circuit Middleware** - Return early based on conditions
- **Conditional Middleware** - Enable middleware based on environment/configuration

---

**💡 Pro Tip**: Middleware order is critical! Always think about the request/response flow. Exception handlers wrap everything, logging happens early, and routing comes last before endpoints!

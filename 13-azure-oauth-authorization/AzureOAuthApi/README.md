# Configuration and Options Pattern in ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Configuration](https://img.shields.io/badge/Configuration-Options_Pattern-FF6B35?style=flat-square)
![Settings](https://img.shields.io/badge/App-Settings-2E8B57?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Configuration and Options Pattern Tutorial](https://youtu.be/YOUR_VIDEO_ID)**

## 🎯 Learning Objectives

By the end of this tutorial, you'll master:
- ✅ **Configuration System** - Understanding ASP.NET Core configuration hierarchy
- ✅ **Options Pattern** - Strongly-typed configuration with `IOptions<T>`
- ✅ **appsettings.json** - Managing configuration files for different environments
- ✅ **Configuration Binding** - Binding JSON configuration to C# classes
- ✅ **Configurable Middleware** - Making middleware behavior configurable
- ✅ **Environment-Specific Settings** - Development vs Production configuration
- ✅ **Configuration Best Practices** - Secure and maintainable configuration management

## 🚀 What We Build

A **Production-Ready Configuration System** featuring:

1. **RequestResponseLoggingOptions** - Strongly-typed configuration class
2. **Configurable Middleware** - Middleware that reads settings from appsettings.json
3. **Environment-Specific Configuration** - Different settings for Development and Production
4. **Options Pattern Implementation** - Dependency injection of configuration settings

## 📁 Project Structure

```
AzureOAuthApi/
├── Controllers/
│   └── ProductsController.cs        # API endpoints
├── Configuration/                    # ⭐ Configuration classes
│   └── RequestResponseLoggingOptions.cs # Strongly-typed options class ⭐
├── Middleware/                       # Custom middleware components
│   ├── RequestLoggingMiddleware.cs  # Logs HTTP requests and responses
│   ├── ResponseTimingMiddleware.cs  # Measures and logs response time
│   └── RequestResponseLoggingMiddleware.cs # Configurable logging middleware ⭐
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
│       └── ProductDtos.cs           # Product DTOs
├── Repositories/
│   ├── IProductRepository.cs        # Repository interface
│   └── ProductRepository.cs         # Repository implementation
├── Services/
│   ├── IProductService.cs           # Service interface
│   ├── ProductService.cs            # Service with validation logic
│   ├── INotificationService.cs      # Notification service interface
│   └── NotificationService.cs       # Notification implementation
├── Data/
│   └── InMemoryDatabase.cs          # In-memory data store
├── Mappings/
│   └── MappingProfile.cs            # AutoMapper configuration
├── Program.cs                       # Configuration and DI setup ⭐
├── appsettings.json                 # Base configuration file ⭐
├── appsettings.Development.json     # Development-specific settings ⭐
└── AzureOAuthApi.http     # HTTP requests for testing
```

## 🏗️ Configuration System Architecture

### **Configuration Hierarchy Flow**

```
┌─────────────────────────────────────────────────────┐
│          Configuration Sources (Priority)           │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│  1. appsettings.json (Base Configuration)           │
│     • Common settings for all environments          │
│     • Default values                                │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│  2. appsettings.{Environment}.json                  │
│     • Environment-specific overrides                │
│     • Development, Staging, Production              │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│  3. Environment Variables (Highest Priority)        │
│     • Container/Cloud configuration                 │
│     • Secrets and sensitive data                    │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│           Configuration Builder                     │
│     • Merges all sources                            │
│     • Later sources override earlier ones           │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│         Options Pattern (IOptions<T>)               │
│     • Strongly-typed configuration classes          │
│     • Dependency injection                          │
│     • Type-safe access to settings                  │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│        Application Components                       │
│     • Middleware                                    │
│     • Services                                      │
│     • Controllers                                   │
└─────────────────────────────────────────────────────┘
```

## 💻 Configuration Implementation

### **Step 1: Create a Strongly-Typed Options Class**

```csharp
// Configuration/RequestResponseLoggingOptions.cs
namespace AzureOAuthApi.Configuration;

/// <summary>
/// Configuration options for the RequestResponseLoggingMiddleware.
/// Demonstrates the Options Pattern for making middleware configurable.
/// </summary>
public class RequestResponseLoggingOptions
{
    /// <summary>
    /// The configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "RequestResponseLogging";

    /// <summary>
    /// Enable or disable detailed request/response logging
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Include request headers in logs
    /// </summary>
    public bool IncludeRequestHeaders { get; set; } = false;

    /// <summary>
    /// Include response headers in logs
    /// </summary>
    public bool IncludeResponseHeaders { get; set; } = false;

    /// <summary>
    /// Include request body in logs
    /// </summary>
    public bool IncludeRequestBody { get; set; } = true;

    /// <summary>
    /// Include response body in logs
    /// </summary>
    public bool IncludeResponseBody { get; set; } = true;

    /// <summary>
    /// Maximum body size to log (in bytes). Bodies larger than this will be truncated.
    /// </summary>
    public int MaxBodySizeToLog { get; set; } = 4096;
}
```

**Key Concepts:**
- **SectionName Constant** - Defines the configuration section name
- **Default Values** - Provides sensible defaults for all properties
- **XML Documentation** - Clear descriptions for each setting
- **Type Safety** - Strongly-typed properties instead of magic strings

---

### **Step 2: Define Configuration in appsettings.json**

```json
// appsettings.json - Base configuration
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "RequestResponseLogging": {
    "IsEnabled": true,
    "IncludeRequestHeaders": false,
    "IncludeResponseHeaders": false,
    "IncludeRequestBody": true,
    "IncludeResponseBody": true,
    "MaxBodySizeToLog": 4096
  }
}
```

```json
// appsettings.Development.json - Development overrides
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "RequestResponseLogging": {
    "IsEnabled": true,
    "IncludeRequestHeaders": true,
    "IncludeResponseHeaders": true,
    "IncludeRequestBody": true,
    "IncludeResponseBody": true,
    "MaxBodySizeToLog": 8192
  }
}
```

```json
// appsettings.Production.json - Production overrides
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "RequestResponseLogging": {
    "IsEnabled": false,
    "IncludeRequestHeaders": false,
    "IncludeResponseHeaders": false,
    "IncludeRequestBody": false,
    "IncludeResponseBody": false,
    "MaxBodySizeToLog": 2048
  }
}
```

**Key Concepts:**
- **Hierarchical Configuration** - Base settings with environment overrides
- **JSON Structure** - Matches the C# class property names
- **Environment-Specific** - Different settings for Development vs Production
- **Security** - Disable verbose logging in Production

---

### **Step 3: Register Options in Program.cs**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// ========================================
// Configure Options Pattern
// ========================================
// Bind configuration sections to strongly-typed options classes
// This demonstrates the Options Pattern for configurable middleware
builder.Services.Configure<RequestResponseLoggingOptions>(
    builder.Configuration.GetSection(RequestResponseLoggingOptions.SectionName));

// Other service registrations...
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register middleware services
builder.Services.AddScoped<RequestResponseLoggingMiddleware>();
builder.Services.AddScoped<ResponseTimingMiddleware>();
builder.Services.AddScoped<RequestLoggingMiddleware>();

var app = builder.Build();

// Configure middleware pipeline
app.UseExceptionHandler();
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // Uses IOptions<T>
app.UseMiddleware<ResponseTimingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();
```

**Key Concepts:**
- **Configure<T>()** - Registers options with the DI container
- **GetSection()** - Retrieves the configuration section by name
- **Strong Typing** - Configuration is bound to `RequestResponseLoggingOptions`
- **Dependency Injection** - Options are injected into components

---

### **Step 4: Consume Options in Middleware**

```csharp
// Middleware/RequestResponseLoggingMiddleware.cs
using Microsoft.Extensions.Options;

public class RequestResponseLoggingMiddleware(
    ILogger<RequestResponseLoggingMiddleware> logger,
    IOptions<RequestResponseLoggingOptions> options)  // ⭐ Inject IOptions<T>
    : IMiddleware
{
    private readonly RequestResponseLoggingOptions _options = options.Value; // ⭐ Get the value

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if logging is enabled via configuration
        if (!_options.IsEnabled)
        {
            await next(context);
            return;
        }

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

        // Copy the contents back to original stream
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task LogRequest(HttpContext context)
    {
        var logBuilder = new StringBuilder();
        logBuilder.AppendLine("HTTP Request Information:");
        logBuilder.AppendLine($"Method: {context.Request.Method}");
        logBuilder.AppendLine($"Path: {context.Request.Path}");

        // Include headers if configured
        if (_options.IncludeRequestHeaders)
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in context.Request.Headers)
            {
                logBuilder.AppendLine($"  {header.Key}: {header.Value}");
            }
        }

        // Include body if configured
        if (_options.IncludeRequestBody)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            // Truncate if body exceeds max size
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [TRUNCATED]";
            }

            logBuilder.AppendLine($"Body: {body}");
        }

        logger.LogInformation(logBuilder.ToString());
    }

    private async Task LogResponse(HttpContext context)
    {
        var logBuilder = new StringBuilder();
        logBuilder.AppendLine("HTTP Response Information:");
        logBuilder.AppendLine($"StatusCode: {context.Response.StatusCode}");

        // Include headers if configured
        if (_options.IncludeResponseHeaders)
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in context.Response.Headers)
            {
                logBuilder.AppendLine($"  {header.Key}: {header.Value}");
            }
        }

        // Include body if configured
        if (_options.IncludeResponseBody)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Truncate if body exceeds max size
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [TRUNCATED]";
            }

            logBuilder.AppendLine($"Body: {body}");
        }

        logger.LogInformation(logBuilder.ToString());
    }
}
```

**Key Concepts:**
- **IOptions<T> Injection** - Options are injected via constructor
- **options.Value** - Access the configured settings
- **Configuration-Driven Behavior** - Middleware behavior changes based on settings
- **Runtime Configuration** - No code changes needed to modify behavior
- **Body Size Limiting** - Prevents logging massive payloads

---

## 🎨 Options Pattern Benefits

### **Before: Hardcoded Configuration** ❌

```csharp
public class RequestResponseLoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Always logs everything - no flexibility
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        logger.LogInformation($"Request Body: {body}");

        await next(context);
    }
}
```

**Problems:**
- ❌ No way to disable logging
- ❌ Always logs full bodies (could be huge!)
- ❌ Can't toggle headers on/off
- ❌ Requires code changes to modify behavior

---

### **After: Options Pattern** ✅

```csharp
public class RequestResponseLoggingMiddleware(
    ILogger<RequestResponseLoggingMiddleware> logger,
    IOptions<RequestResponseLoggingOptions> options) : IMiddleware
{
    private readonly RequestResponseLoggingOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if enabled via configuration
        if (!_options.IsEnabled)
        {
            await next(context);
            return;
        }

        // Log only if configured
        if (_options.IncludeRequestBody)
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

            // Truncate if needed
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [TRUNCATED]";
            }

            logger.LogInformation($"Request Body: {body}");
        }

        await next(context);
    }
}
```

**Benefits:**
- ✅ Enable/disable via appsettings.json
- ✅ Body size limits to prevent huge logs
- ✅ Toggle headers/body independently
- ✅ Environment-specific configuration
- ✅ No code changes needed

---

## 🔧 Configuration Patterns

### **Pattern 1: Simple Configuration Binding**

```csharp
// Register options
builder.Services.Configure<MyOptions>(
    builder.Configuration.GetSection("MySection"));

// Consume in service
public class MyService(IOptions<MyOptions> options)
{
    private readonly MyOptions _options = options.Value;
}
```

### **Pattern 2: Configuration with Validation**

```csharp
// Options class with validation
public class ApiKeyOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

// Register with validation
builder.Services.AddOptions<ApiKeyOptions>()
    .Bind(builder.Configuration.GetSection("ApiKey"))
    .Validate(options => !string.IsNullOrEmpty(options.ApiKey),
              "API Key is required");
```

### **Pattern 3: Multiple Configuration Sources**

```csharp
// Build configuration from multiple sources
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()  // Development secrets
    .Build();
```

### **Pattern 4: IOptionsSnapshot for Reloadable Configuration**

```csharp
// Use IOptionsSnapshot instead of IOptions for reloadable config
public class MyService(IOptionsSnapshot<MyOptions> options)
{
    // Options are re-evaluated on each request if config file changes
    private MyOptions GetCurrentOptions() => options.Value;
}
```

## 🧪 Testing Configuration

### **Test Different Configuration Values**

```http
### Test with IsEnabled = true (Default)
GET https://localhost:7xxx/api/products
```

**Expected:** Full request/response logging in console

---

```http
### Change appsettings.json: IsEnabled = false
GET https://localhost:7xxx/api/products
```

**Expected:** No detailed logging

---

```http
### Change IncludeRequestHeaders = true
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Test Product",
  "price": 99.99
}
```

**Expected:** Headers included in logs

---

## 🎓 Key Benefits

### **1. Type Safety**
- ✅ Strongly-typed configuration classes
- ✅ Compile-time checking
- ✅ IntelliSense support
- ✅ Refactoring safety

### **2. Maintainability**
- ✅ Centralized configuration
- ✅ Clear structure with XML docs
- ✅ Default values in code
- ✅ Easy to understand and modify

### **3. Environment Management**
- ✅ Base settings with overrides
- ✅ Development vs Production configs
- ✅ Container/Cloud ready
- ✅ Secret management support

### **4. Flexibility**
- ✅ Change behavior without code changes
- ✅ Toggle features on/off
- ✅ Adjust limits and thresholds
- ✅ Runtime configuration updates (with IOptionsSnapshot)

### **5. Testability**
- ✅ Easy to mock IOptions<T>
- ✅ Inject test configurations
- ✅ Unit test with different settings
- ✅ Integration test environment configs

---

## 🔧 Running the Project

```bash
cd 13-azure-oauth-authorization/AzureOAuthApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`

### **Testing Different Environments**

```bash
# Run with Development environment (uses appsettings.Development.json)
dotnet run --environment Development

# Run with Production environment (uses appsettings.Production.json)
dotnet run --environment Production

# Run with custom environment
dotnet run --environment Staging
```

---

## 🎯 Key Takeaways

1. **Options Pattern**: Use `IOptions<T>` for strongly-typed configuration
2. **Configuration Hierarchy**: appsettings.json → appsettings.{Environment}.json → Environment Variables
3. **Type Safety**: Configuration classes provide compile-time checking
4. **Environment-Specific**: Different settings for Development, Staging, Production
5. **Dependency Injection**: Options are injected into services and middleware
6. **Reloadable Config**: Use `IOptionsSnapshot<T>` for configs that change at runtime
7. **Validation**: Add configuration validation for critical settings
8. **Security**: Store secrets in environment variables or Key Vault, not in appsettings.json

---

## 🔒 Configuration Security Best Practices

### **❌ Never Store Secrets in appsettings.json**

```json
// DON'T DO THIS!
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod;Database=MyDb;User=admin;Password=Pa$$w0rd123"
  },
  "ApiKeys": {
    "PaymentGateway": "sk_live_abc123xyz789"
  }
}
```

### **✅ Use Environment Variables or Secret Managers**

```csharp
// Development: User Secrets
dotnet user-secrets init
dotnet user-secrets set "ApiKeys:PaymentGateway" "sk_test_abc123"

// Production: Environment Variables
// Set in Azure App Service, Kubernetes, Docker, etc.
export ApiKeys__PaymentGateway="sk_live_xyz789"
```

```csharp
// Access secrets the same way as normal config
builder.Services.Configure<ApiKeyOptions>(
    builder.Configuration.GetSection("ApiKeys"));
```

---

## ➡️ What's Next?

**Extend this configuration system with:**
- **Azure Key Vault** - Store secrets in Azure Key Vault
- **Configuration Validation** - Validate settings at startup
- **IOptionsSnapshot** - Reloadable configuration without restart
- **IOptionsMonitor** - Track configuration changes with callbacks
- **Custom Configuration Providers** - Load config from database, APIs, etc.
- **Feature Flags** - Toggle features dynamically
- **Configuration Encryption** - Encrypt sensitive sections

---

## 💡 Pro Tips

1. **Use const for Section Names** - Prevents typos and enables refactoring
   ```csharp
   public const string SectionName = "RequestResponseLogging";
   ```

2. **Provide Default Values** - Initialize properties with sensible defaults
   ```csharp
   public bool IsEnabled { get; set; } = false;
   ```

3. **Document Configuration** - Use XML comments for all options
   ```csharp
   /// <summary>
   /// Maximum body size to log (in bytes)
   /// </summary>
   public int MaxBodySizeToLog { get; set; } = 4096;
   ```

4. **Validate Configuration** - Add startup validation for critical settings
   ```csharp
   builder.Services.AddOptions<MyOptions>()
       .Validate(o => o.MaxSize > 0, "MaxSize must be positive");
   ```

5. **Environment-Specific Configs** - Use different settings for each environment
   - Development: Verbose logging, debug mode
   - Production: Minimal logging, optimized settings

---

**💡 Pro Tip**: The Options Pattern is the recommended way to access configuration in ASP.NET Core. It provides type safety, dependency injection, and flexibility without sacrificing performance!

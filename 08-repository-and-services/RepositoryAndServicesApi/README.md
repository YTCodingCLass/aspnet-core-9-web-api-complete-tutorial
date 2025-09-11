# Chapter 07: Dependency Injection - Understanding Service Lifetimes

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Dependency Injection](https://img.shields.io/badge/Dependency_Injection-DI-FF6B35?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Chapter 07: Dependency Injection](#)** *(Add your video link here)*

## 🎯 Learning Objectives

By the end of this chapter, you'll master:
- ✅ Understanding Dependency Injection principles in ASP.NET Core
- ✅ Creating and registering services in the DI container
- ✅ Service lifetimes: **Transient**, **Scoped**, and **Singleton**
- ✅ Practical differences between service lifetimes
- ✅ When to use each service lifetime in real applications

## 🚀 What We Build

A **Service Lifetime Demo API** that demonstrates:
- **Singleton Services** - Same instance across entire application
- **Scoped Services** - Same instance within a single HTTP request
- **Transient Services** - New instance every time it's requested
- **Visual demonstration** of how each lifetime behaves
- **Console logging** to track instance creation

## 📁 Project Structure

```
RepositoryAndServicesApi/
├── Controllers/
│   ├── ProductsController.cs        # Products API with AutoMapper
│   └── ServiceLifetimeController.cs # Service lifetime demonstration
├── Models/
│   ├── Product.cs              # Product entity model
│   └── DTOs/
│       ├── CreateProductDto.cs     # Request DTOs
│       ├── UpdateProductDto.cs     
│       ├── PatchProductDto.cs      
│       └── ProductResponseDto.cs   # Response DTO with stock status
├── Services.cs                 # Service interfaces and implementations
├── Mappings/
│   └── MappingProfile.cs       # AutoMapper configuration
├── Program.cs                  # DI container registration
└── RepositoryAndServicesApi.http # HTTP requests for testing
```

## 🔧 Understanding Service Lifetimes

### **1. Service Interfaces and Implementations**
```csharp
// Services.cs
namespace RepositoryAndServicesApi;

// Interface definitions
public interface ISingletonService
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
}

public interface IScopedService  
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
}

public interface ITransientService
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
}

// Service implementations with logging
public class SingletonService : ISingletonService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public SingletonService()
    {
        Console.WriteLine($"🔴 Singleton created: {InstanceId}");
    }
}

public class ScopedService : IScopedService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public ScopedService()
    {
        Console.WriteLine($"🟡 Scoped created: {InstanceId}");
    }
}

public class TransientService : ITransientService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public TransientService()
    {
        Console.WriteLine($"🟢 Transient created: {InstanceId}");
    }
}
```

### **2. Register Services in DI Container**
```csharp
// Program.cs
builder.Services.AddSingleton<ISingletonService, SingletonService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddTransient<ITransientService, TransientService>();
```

## 💻 Service Lifetime Controller

### **Demonstrating Service Lifetimes**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ServiceLifetimeController : ControllerBase
{
    // Inject TWO instances of each service type
    private readonly ISingletonService singletonService1;
    private readonly ISingletonService singletonService2;
    private readonly IScopedService scopedService1;
    private readonly IScopedService scopedService2;
    private readonly ITransientService transientService1;
    private readonly ITransientService transientService2;
    
    public ServiceLifetimeController(
        ISingletonService singleton1,
        ISingletonService singleton2,
        IScopedService scoped1,
        IScopedService scoped2,
        ITransientService transient1,
        ITransientService transient2)
    {
        singletonService1 = singleton1;
        singletonService2 = singleton2;
        scopedService1 = scoped1;
        scopedService2 = scoped2;
        transientService1 = transient1;
        transientService2 = transient2;
        
        Console.WriteLine("🏗️ Controller created with all services");
    }
    
    [HttpGet("demo")]
    public ActionResult<object> GetServiceLifetimeDemo()
    {
        return Ok(new
        {
            Explanation = new
            {
                Singleton = "Same instance across entire application lifetime",
                Scoped = "Same instance within a single HTTP request", 
                Transient = "New instance every time service is requested"
            },
            Results = new
            {
                Singleton = new
                {
                    Instance1_Id = singletonService1.InstanceId,
                    Instance2_Id = singletonService2.InstanceId,
                    AreSame = singletonService1.InstanceId == singletonService2.InstanceId
                },
                Scoped = new
                {
                    Instance1_Id = scopedService1.InstanceId,
                    Instance2_Id = scopedService2.InstanceId,
                    AreSame = scopedService1.InstanceId == scopedService2.InstanceId
                },
                Transient = new
                {
                    Instance1_Id = transientService1.InstanceId,
                    Instance2_Id = transientService2.InstanceId,
                    AreSame = transientService1.InstanceId == transientService2.InstanceId
                }
            }
        });
    }
}
```

## 🎛️ Service Lifetime Behavior

### **🔴 Singleton Lifetime**
- **Created once** when first requested
- **Same instance** shared across entire application
- **Lives until** application shuts down
- **Use for**: Stateless services, caching, configuration

```csharp
builder.Services.AddSingleton<ISingletonService, SingletonService>();
```

### **🟡 Scoped Lifetime**
- **Created once** per HTTP request
- **Same instance** within the same request
- **Disposed** when request ends
- **Use for**: Database contexts, request-specific services

```csharp
builder.Services.AddScoped<IScopedService, ScopedService>();
```

### **🟢 Transient Lifetime**
- **Created every time** service is requested
- **New instance** for each injection
- **Disposed** when scope ends
- **Use for**: Lightweight, stateless services

```csharp
builder.Services.AddTransient<ITransientService, TransientService>();
```

## 🧪 Testing Service Lifetimes

### **Test Service Lifetime Demo**
```http
GET https://localhost:7xxx/api/servicelifetime/demo
```

### **Expected Response**
```json
{
  "explanation": {
    "singleton": "Same instance across entire application lifetime",
    "scoped": "Same instance within a single HTTP request",
    "transient": "New instance every time service is requested"
  },
  "results": {
    "singleton": {
      "instance1_Id": "550e8400-e29b-41d4-a716-446655440000",
      "instance2_Id": "550e8400-e29b-41d4-a716-446655440000",
      "areSame": true
    },
    "scoped": {
      "instance1_Id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
      "instance2_Id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
      "areSame": true
    },
    "transient": {
      "instance1_Id": "6ba7b811-9dad-11d1-80b4-00c04fd430c8",
      "instance2_Id": "6ba7b812-9dad-11d1-80b4-00c04fd430c8",
      "areSame": false
    }
  },
  "requestTime": "2025-08-16T10:30:00Z"
}
```

### **Console Output**
```
🔴 Singleton created: 550e8400-e29b-41d4-a716-446655440000
🟡 Scoped created: 6ba7b810-9dad-11d1-80b4-00c04fd430c8
🟢 Transient created: 6ba7b811-9dad-11d1-80b4-00c04fd430c8
🟢 Transient created: 6ba7b812-9dad-11d1-80b4-00c04fd430c8
🏗️ Controller created with all services
```

## 🎓 Key Observations

### **1. Singleton Behavior**
- ✅ **Instance1_Id == Instance2_Id** (Same GUID)
- ✅ **Created only once** during application startup
- ✅ **Shared across all requests** and controllers

### **2. Scoped Behavior**
- ✅ **Instance1_Id == Instance2_Id** within same request
- ✅ **New instance per HTTP request**
- ✅ **Disposed when request completes**

### **3. Transient Behavior**
- ❌ **Instance1_Id != Instance2_Id** (Different GUIDs)
- ✅ **New instance every injection**
- ✅ **Two different instances** in same controller

### **4. Performance Implications**
- **Singleton**: Fastest, no allocation overhead
- **Scoped**: Moderate, one allocation per request
- **Transient**: Slowest, allocation on every injection

## 🔧 Running the Project

```bash
cd 07-dependency-injection/RepositoryAndServicesApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Service Demo**: `https://localhost:7xxx/api/servicelifetime/demo`

## 🏗️ Real-World Usage Guidelines

### **When to Use Singleton**
```csharp
// ✅ Good candidates
builder.Services.AddSingleton<IConfiguration>();
builder.Services.AddSingleton<ILogger<T>>();
builder.Services.AddSingleton<ICacheService>();
builder.Services.AddSingleton<IEmailService>();
```

### **When to Use Scoped**
```csharp
// ✅ Good candidates
builder.Services.AddScoped<DbContext>();
builder.Services.AddScoped<IUserService>();
builder.Services.AddScoped<IOrderService>();
builder.Services.AddScoped<IUnitOfWork>();
```

### **When to Use Transient**
```csharp
// ✅ Good candidates
builder.Services.AddTransient<IValidator<T>>();
builder.Services.AddTransient<IMapper>();
builder.Services.AddTransient<IHttpClientFactory>();
builder.Services.AddTransient<IDateTimeProvider>();
```

### **⚠️ Common Pitfalls**
- **Don't** inject Scoped services into Singletons
- **Don't** store state in Transient services
- **Don't** use Transient for expensive-to-create objects

## 📈 Performance Impact Demonstration

### **Experiment: Multiple Requests**
1. **First Request** - All services created:
```
🔴 Singleton created: [GUID-1]
🟡 Scoped created: [GUID-2] 
🟢 Transient created: [GUID-3]
🟢 Transient created: [GUID-4]
```

2. **Second Request** - Only Scoped and Transient created:
```
🟡 Scoped created: [GUID-5]  // New for this request
🟢 Transient created: [GUID-6]  // New instances
🟢 Transient created: [GUID-7]
```

3. **Third Request** - Pattern continues:
```
🟡 Scoped created: [GUID-8]  // New for this request
🟢 Transient created: [GUID-9]  // Always new
🟢 Transient created: [GUID-10]
```

### **Memory and CPU Impact**
- **Singleton**: Zero allocation after first request
- **Scoped**: One allocation per request
- **Transient**: Multiple allocations per request

## 🔍 Debugging Service Lifetimes

### **Common Issues and Solutions**

**1. Captive Dependency Problem**
```csharp
// ❌ BAD: Singleton capturing Scoped service
public class MySingleton
{
    private readonly DbContext _context; // Scoped!
    // This will cause issues!
}

// ✅ GOOD: Use IServiceProvider or factory pattern
public class MySingleton
{
    private readonly IServiceProvider _serviceProvider;
    
    public void DoSomething()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        // Use context safely
    }
}
```

**2. Debugging Service Creation**
```csharp
// Add logging to constructors
public MyService(ILogger<MyService> logger)
{
    logger.LogInformation("MyService created: {InstanceId}", Guid.NewGuid());
}
```

## 🎯 Key Takeaways

1. **Singleton**: One instance for entire application lifetime
2. **Scoped**: One instance per HTTP request
3. **Transient**: New instance every time it's injected
4. **Choose lifetime based on state management and performance needs**
5. **Be careful with captive dependencies**
6. **Use console logging to visualize instance creation**

## ➡️ What's Next?

**Next learning paths:**
- **Repository Pattern** with Dependency Injection
- **Entity Framework Core** with proper service registration
- **Unit Testing** with mocked dependencies
- **Authentication & Authorization** services
- **Background Services** and hosted services

## 🤔 Troubleshooting

**Service not found exception?**
- Check service registration in Program.cs
- Verify interface and implementation are correctly registered

**Captive dependency detected?**
- Don't inject shorter-lived services into longer-lived ones
- Use factory pattern or service locator for complex scenarios

**Memory leaks with services?**
- Implement IDisposable for services that hold resources
- Be careful with event subscriptions in singleton services

---

**💡 Pro Tip**: Use the `/demo` endpoint to understand service behavior before building complex applications!
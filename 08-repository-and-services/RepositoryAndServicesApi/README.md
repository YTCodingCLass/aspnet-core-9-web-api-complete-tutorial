# Repository and Service Pattern with Dependency Injection

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Repository Pattern](https://img.shields.io/badge/Repository_Pattern-Architecture-FF6B35?style=flat-square)
![Service Layer](https://img.shields.io/badge/Service_Layer-Business_Logic-2E8B57?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Repository and Service Pattern Tutorial](https://www.youtube.com/watch?v=ZoW5RnvgeSo&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=2)**

## 🎯 Learning Objectives

By the end of this tutorial, you'll master:
- ✅ **Repository Pattern** - Abstracting data access layer from business logic
- ✅ **Service Layer Architecture** - Implementing business logic and service coordination
- ✅ **Dependency Injection** - Managing component dependencies and lifetimes
- ✅ **DTO Pattern** - Separating internal models from API contracts
- ✅ **AutoMapper Integration** - Automated object-to-object mapping
- ✅ **Clean Architecture** - Building maintainable, testable, and scalable APIs

## 🚀 What We Build

A **Product Management API** that demonstrates:
- **Repository Pattern** - Clean data access abstraction
- **Service Layer** - Business logic and validation
- **Dependency Injection** - Proper component wiring
- **AutoMapper Integration** - DTO mapping automation
- **Notification Service** - Cross-cutting concerns

## 📁 Project Structure

```
RepositoryAndServicesApi/
├── Controllers/
│   ├── ProductsController.cs        # Products API controller
│   └── ServiceLifetimeController.cs # Service lifetime demonstration
├── Models/
│   ├── Product.cs                   # Product entity model
│   ├── Supplier.cs                  # Supplier entity model
│   └── DTOs/
│       ├── CreateProductDto.cs      # Create product request DTO
│       ├── UpdateProductDto.cs      # Update product request DTO
│       ├── PatchProductDto.cs       # Patch product request DTO
│       └── ProductResponseDto.cs    # Product response DTO with stock status
├── Repositories/
│   ├── IProductRepository.cs        # Repository interface for data access
│   └── ProductRepository.cs         # Repository implementation
├── Services/
│   ├── IProductService.cs           # Service interface for business logic
│   ├── ProductService.cs            # Service implementation
│   ├── INotificationService.cs      # Notification service interface
│   └── NotificationService.cs       # Notification service implementation
├── Data/
│   └── InMemoryDatabase.cs          # In-memory data store
├── Services/
│   └── BusinessException.cs         # Business logic exceptions (moved to Services folder)
├── Mappings/
│   └── MappingProfile.cs            # AutoMapper configuration
├── Program.cs                       # DI container registration and app configuration
└── RepositoryAndServicesApi.http    # HTTP requests for testing
```

## 🏗️ Architecture Overview

The architecture follows a clean layered approach as illustrated in the diagram:

```
┌─────────────────────────────────────────────────┐
│                  REST APIs                      │
├─────────────────────────────────────────────────┤
│              REST CONTROLLER                    │
├─────────┬─────────────────────────────┬─────────┤
│  DTOs   │       SERVICES              │ENTITIES │
│         │         +                   │         │
│         │       MAPPERS               │         │
├─────────┴─────────────────────────────┴─────────┤
│              JPA REPOSITORY                     │
├─────────────────────────────────────────────────┤
│                  Database                       │
└─────────────────────────────────────────────────┘
```

### **Layer Responsibilities:**
- **REST Controller**: HTTP endpoints, request/response handling, validation
- **Services**: Business logic, data transformation, cross-cutting concerns
- **Mappers**: Object mapping between DTOs and Entities using AutoMapper
- **Repository**: Data access abstraction, CRUD operations
- **DTOs**: Data transfer objects for API contracts
- **Entities**: Domain models representing business data

### **1. Repository Pattern Implementation**
```csharp
// Repositories/IProductRepository.cs - Data Access Abstraction
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByIdWithSupplierAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

// Repositories/ProductRepository.cs - Data Access Implementation
public class ProductRepository : IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ILogger<ProductRepository> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        _logger.LogInformation("🗃️ Repository: Getting all products");
        
        return InMemoryDatabase.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToList();
    }
    
    // Other repository methods...
}
```

### **2. Service Layer Implementation**
```csharp
// Services/IProductService.cs - Business Logic Interface
public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<ProductResponseDto?> GetProductByIdAsync(int id);
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto);
    Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
    Task<bool> DeleteProductAsync(int id);
}

// Services/ProductService.cs - Business Logic Implementation
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        _logger.LogInformation("⚙️ Service: Getting all products");
        
        var products = await _productRepository.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        // Business logic: Calculate stock status
        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }
        
        return productDtos;
    }
    
    // Business logic methods...
    private string CalculateStockStatus(int stockQuantity)
    {
        return stockQuantity switch
        {
            0 => "Out of Stock",
            <= 5 => "Critical Low",
            <= 10 => "Low Stock",
            <= 50 => "Normal",
            _ => "Well Stocked"
        };
    }
}
```

### **3. Dependency Injection Registration**
```csharp
// Program.cs - Service Registration
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

## 🎮 Controller Implementation

### **Products Controller with Clean Architecture**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
    {
        _logger.LogInformation("🎮 Controller: Getting all products");
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
    {
        _logger.LogInformation("🎮 Controller: Getting product {ProductId}", id);
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found");
        }
        
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct(CreateProductDto createProductDto)
    {
        _logger.LogInformation("🎮 Controller: Creating product {ProductName}", createProductDto.Name);
        
        try
        {
            var newProduct = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        _logger.LogInformation("🎮 Controller: Updating product {ProductId}", id);
        
        try
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);
            if (updatedProduct == null)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return Ok(updatedProduct);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("🎮 Controller: Deleting product {ProductId}", id);
        
        try
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return NoContent();
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
```

## 🏛️ Architecture Benefits

### **🗃️ Repository Pattern Benefits**
- **Data Access Abstraction** - Hide data storage implementation details
- **Testability** - Easy to mock data access for unit tests
- **Flexibility** - Switch between different data sources without changing business logic
- **Single Responsibility** - Repositories only handle data access operations

```csharp
// Easy to switch from in-memory to database
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // In-memory
// builder.Services.AddScoped<IProductRepository, SqlProductRepository>(); // SQL Server
// builder.Services.AddScoped<IProductRepository, MongoProductRepository>(); // MongoDB
```

### **⚙️ Service Layer Benefits**
- **Business Logic Encapsulation** - All business rules in one place
- **Cross-cutting Concerns** - Handle notifications, validation, logging
- **DTO Transformation** - Convert between domain models and DTOs
- **Transaction Management** - Coordinate multiple repository operations

```csharp
// Service coordinates multiple operations
public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto)
{
    // 1. Validate business rules
    await ValidateProductCreationAsync(createDto);
    
    // 2. Transform DTO to domain model
    var product = _mapper.Map<Product>(createDto);
    
    // 3. Save to repository
    var createdProduct = await _productRepository.CreateAsync(product);
    
    // 4. Send notifications
    await _notificationService.SendProductCreatedNotificationAsync(responseDto);
    
    // 5. Return response DTO
    return _mapper.Map<ProductResponseDto>(createdProduct);
}
```

### **🔗 Dependency Injection Benefits**
- **Loose Coupling** - Components depend on abstractions, not implementations
- **Inversion of Control** - Framework manages object lifecycle and dependencies
- **Configuration Flexibility** - Easy to change implementations via registration
- **Scoped Lifetime Management** - Proper resource management and disposal

```csharp
// Clean separation of concerns through DI
builder.Services.AddScoped<IProductRepository, ProductRepository>();    // Data Access
builder.Services.AddScoped<IProductService, ProductService>();          // Business Logic
builder.Services.AddScoped<INotificationService, NotificationService>(); // Cross-cutting
builder.Services.AddAutoMapper(typeof(MappingProfile));                 // Object Mapping
```

## 🧪 Testing the API

### **Product CRUD Operations**

#### **1. Get All Products**
```http
GET https://localhost:7xxx/api/products
```

#### **2. Get Product by ID**
```http
GET https://localhost:7xxx/api/products/1
```

#### **3. Create New Product**
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Wireless Headphones",
  "price": 99.99,
  "costPrice": 50.00,
  "category": "Electronics",
  "stockQuantity": 25,
  "supplierId": 1
}
```

#### **4. Update Existing Product**
```http
PUT https://localhost:7xxx/api/products/1
Content-Type: application/json

{
  "name": "Updated Laptop Pro",
  "price": 1299.99,
  "costPrice": 800.00,
  "category": "Electronics",
  "stockQuantity": 15,
  "supplierId": 2
}
```

#### **5. Delete Product**
```http
DELETE https://localhost:7xxx/api/products/1
```

### **Sample API Response**
```json
{
  "id": 1,
  "name": "Laptop Pro",
  "price": 1199.99,
  "category": "Electronics",
  "stockQuantity": 10,
  "stockStatus": "Low Stock",
  "supplier": {
    "id": 1,
    "name": "Tech Solutions Inc.",
    "email": "contact@techsolutions.com"
  },
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T14:45:00Z"
}
```

### **Console Logging Output**
```
🗃️ Repository: Getting all products
⚙️ Service: Getting all products
⚙️ Service: Retrieved 5 products
🎮 Controller: Getting all products

🗃️ Repository: Creating new product: Wireless Headphones  
⚙️ Service: Creating new product: Wireless Headphones
📧 Notification: Product created - Wireless Headphones
🎮 Controller: Creating product Wireless Headphones
```

## 🎓 Key Architecture Patterns

### **1. Repository Pattern Implementation**
- ✅ **Data Access Abstraction** - `IProductRepository` hides data storage details
- ✅ **Async Operations** - All repository methods are asynchronous
- ✅ **Entity Relationships** - `GetByIdWithSupplierAsync` loads related entities
- ✅ **Filtering Support** - Methods for category and low-stock filtering

### **2. Service Layer Implementation**
- ✅ **Business Logic Encapsulation** - Stock status calculation, validation rules
- ✅ **DTO Mapping** - Automatic conversion between entities and DTOs
- ✅ **Cross-cutting Concerns** - Notifications, logging, exception handling
- ✅ **Validation** - Business rule enforcement (profit margins, duplicate names)

### **3. Dependency Injection Benefits**
- ✅ **Loose Coupling** - Controller → Service → Repository chain
- ✅ **Testability** - Easy to mock dependencies for unit testing
- ✅ **Single Responsibility** - Each component has one clear purpose
- ✅ **Configuration Flexibility** - Easy to swap implementations

### **4. Clean Architecture Flow**
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   DTOs      │◄──►│ Controller  │────│   Service   │────│ Repository  │
│             │    │             │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                           │                   │                   │
                           ▼                   ▼                   ▼
                   HTTP Handling      Business Logic      Data Access
                   Input Validation   Data Transformation Query Operations
                   Error Responses    Cross-cutting       Entity Mapping
                                     Service Coordination
                                           │
                                           ▼
                                   ┌─────────────┐    ┌─────────────┐
                                   │   Mappers   │◄──►│  Entities   │
                                   │(AutoMapper) │    │ (Models)    │
                                   └─────────────┘    └─────────────┘
```

## 🔧 Running the Project

```bash
cd RepositoryAndServicesApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`
**Service Demo**: `https://localhost:7xxx/api/servicelifetime/demo`

## 🏗️ Implementation Best Practices

### **Repository Pattern Guidelines**
```csharp
// ✅ DO: Keep repositories focused on data access
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();           // Query operations
    Task<Product> CreateAsync(Product product);         // Command operations
    Task<bool> ExistsAsync(int id);                    // Utility operations
}

// ❌ DON'T: Put business logic in repositories
public interface IProductRepository
{
    Task<decimal> CalculateTotalValue();  // Business logic belongs in service
    Task SendLowStockAlert();            // Cross-cutting concern belongs in service
}
```

### **Service Layer Guidelines**
```csharp
// ✅ DO: Encapsulate business logic in services
public class ProductService : IProductService
{
    // Business validation
    private async Task ValidateProductCreationAsync(CreateProductDto dto) { }
    
    // Business calculations
    private string CalculateStockStatus(int stock) { }
    
    // Coordinate multiple operations
    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto) { }
}

// ❌ DON'T: Put data access logic in services
public class ProductService : IProductService
{
    public async Task<Product> GetProductAsync(int id)
    {
        // Don't write SQL or data access code here
        var sql = "SELECT * FROM Products WHERE Id = @id";
        return await _connection.QueryAsync<Product>(sql, new { id });
    }
}
```

### **Dependency Injection Guidelines**
```csharp
// ✅ DO: Use appropriate lifetimes
builder.Services.AddScoped<IProductRepository, ProductRepository>();    // Per-request
builder.Services.AddScoped<IProductService, ProductService>();          // Per-request
builder.Services.AddSingleton<INotificationService, NotificationService>(); // Stateless
builder.Services.AddTransient<IValidator<CreateProductDto>, ProductValidator>(); // Lightweight

// ❌ DON'T: Mix lifetimes incorrectly
builder.Services.AddSingleton<IProductService, ProductService>(); // BAD: Service depends on scoped repo
```

### **⚠️ Common Anti-patterns**
- **Don't** put business logic in controllers - use services
- **Don't** access repositories directly from controllers - use services
- **Don't** return domain entities from controllers - use DTOs
- **Don't** catch exceptions in repositories - let services handle them

## 🔍 Unit Testing with Repository and Service Pattern

### **Repository Testing with Mocks**
```csharp
[Test]
public async Task GetAllAsync_ShouldReturnActiveProducts()
{
    // Arrange
    var mockLogger = new Mock<ILogger<ProductRepository>>();
    var repository = new ProductRepository(mockLogger.Object);

    // Act
    var products = await repository.GetAllAsync();

    // Assert
    Assert.That(products, Is.Not.Null);
    Assert.That(products.All(p => p.IsActive), Is.True);
}
```

### **Service Testing with Mock Repository**
```csharp
[Test]
public async Task CreateProductAsync_ShouldValidateBusinessRules()
{
    // Arrange
    var mockRepository = new Mock<IProductRepository>();
    var mockNotificationService = new Mock<INotificationService>();
    var mockMapper = new Mock<IMapper>();
    var mockLogger = new Mock<ILogger<ProductService>>();
    
    var service = new ProductService(
        mockRepository.Object,
        mockNotificationService.Object, 
        mockMapper.Object,
        mockLogger.Object);

    var createDto = new CreateProductDto 
    { 
        Name = "Test Product", 
        Price = 50.00m, 
        CostPrice = 60.00m // Invalid: Price lower than cost
    };

    // Act & Assert
    var ex = await Assert.ThrowsAsync<BusinessException>(
        () => service.CreateProductAsync(createDto));
    
    Assert.That(ex.Message, Contains.Substring("10% higher than cost price"));
}
```

### **Controller Testing with Mock Service**
```csharp
[Test]
public async Task GetProducts_ShouldReturnOkWithProducts()
{
    // Arrange
    var mockService = new Mock<IProductService>();
    var mockLogger = new Mock<ILogger<ProductsController>>();
    var controller = new ProductsController(mockService.Object, mockLogger.Object);

    var expectedProducts = new List<ProductResponseDto> 
    {
        new ProductResponseDto { Id = 1, Name = "Test Product" }
    };
    
    mockService.Setup(s => s.GetAllProductsAsync())
               .ReturnsAsync(expectedProducts);

    // Act
    var result = await controller.GetProducts();

    // Assert
    Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    var okResult = result.Result as OkObjectResult;
    Assert.That(okResult.Value, Is.EqualTo(expectedProducts));
}
```

## 🎯 Key Takeaways

1. **Repository Pattern**: Abstracts data access and makes testing easier
2. **Service Layer**: Encapsulates business logic and coordinates operations  
3. **Dependency Injection**: Enables loose coupling and flexible configuration
4. **DTO Pattern**: Separates internal models from API contracts
5. **Clean Architecture**: Each layer has single responsibility and clear boundaries

## ➡️ What's Next?

**Extend this architecture with:**
- **Entity Framework Core** - Replace in-memory data with real database
- **Unit of Work Pattern** - Manage transactions across multiple repositories
- **CQRS Pattern** - Separate read and write operations
- **MediatR** - Implement request/response pattern with handlers
- **Background Services** - Add async processing for notifications

## 🤔 Troubleshooting

**Service registration issues?**
- Verify all interfaces and implementations are registered in `Program.cs`
- Check that service lifetimes are compatible (don't inject scoped into singleton)

**AutoMapper configuration problems?**
- Ensure `MappingProfile` is registered: `builder.Services.AddAutoMapper(typeof(MappingProfile))`
- Verify all DTO-Entity mappings are configured properly

**Business validation not working?**
- Check that `BusinessException` is properly thrown from service layer
- Ensure controller catches `BusinessException` and returns appropriate HTTP response

**Repository returning null?**
- Verify entity exists and `IsActive` flag is true (soft delete implementation)
- Check that async methods are properly awaited

---

**💡 Pro Tip**: Start with this clean architecture foundation and gradually add complexity as your application grows!
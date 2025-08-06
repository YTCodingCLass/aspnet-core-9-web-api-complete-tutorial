# Chapter 06: AutoMapper Integration - Eliminating Boilerplate Code

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![AutoMapper](https://img.shields.io/badge/AutoMapper-Mapping-FF6B35?style=flat-square)

## 📺 YouTube Video
**🔗 [Watch Chapter 06: AutoMapper Integration](#)** *(Add your video link here)*

## 🎯 Learning Objectives

By the end of this chapter, you'll master:
- ✅ Installing and configuring AutoMapper in ASP.NET Core
- ✅ Creating mapping profiles for DTOs and entities
- ✅ Eliminating manual mapping boilerplate code
- ✅ Advanced mapping scenarios and custom configurations
- ✅ Best practices for object mapping in Web APIs

## 🚀 What We Build

Enhanced Product API with AutoMapper:
- **Automatic mapping** between DTOs and entities
- **Mapping profiles** for organized configuration
- **Custom mapping rules** for complex scenarios
- **Cleaner controllers** with reduced boilerplate

## 📁 Project Structure

```
AutoMapper/
├── Controllers/
│   └── ProductsController.cs    # Clean controllers with AutoMapper
├── Models/
│   ├── Product.cs              # Entity model
│   ├── CreateProductDto.cs     # Request DTOs
│   ├── UpdateProductDto.cs     
│   ├── PatchProductDto.cs      
│   └── ProductResponseDto.cs   # Response DTO
├── Mapping/
│   └── ProductMappingProfile.cs # AutoMapper configuration
├── Program.cs                  # AutoMapper DI registration
└── AutoMapper.http             # HTTP requests
```

## 🔧 AutoMapper Setup

### **1. Install AutoMapper Package**
```bash
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

### **2. Create Mapping Profile**
```csharp
// Mapping/ProductMappingProfile.cs
using AutoMapper;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Entity to Response DTO
        CreateMap<Product, ProductResponseDto>();
        
        // Create DTO to Entity
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.InternalNotes, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));
            
        // Update DTO to Entity
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            
        // Patch DTO to Entity (conditional mapping)
        CreateMap<PatchProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, member) => member != null));
    }
}
```

### **3. Register AutoMapper in DI Container**
```csharp
// Program.cs
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));
```

## 💻 Before vs After AutoMapper

### **Before - Manual Mapping (Chapter 05)**
```csharp
[HttpPost]
public ActionResult<ProductResponseDto> CreateProduct(CreateProductDto createDto)
{
    // ❌ Manual mapping - repetitive and error-prone
    var product = new Product
    {
        Name = createDto.Name,
        Price = createDto.Price,
        Category = createDto.Category,
        Id = products.Max(x => x.Id) + 1,
        CreatedAt = DateTime.UtcNow,
        InternalNotes = string.Empty,
        IsDeleted = false,
        CreatedByUserId = 1,
        CostPrice = 0
    };

    products.Add(product);
    
    // Manual response mapping
    var responseDto = new ProductResponseDto
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Category = product.Category,
        CreatedAt = product.CreatedAt
    };
    
    return Ok(responseDto);
}
```

### **After - AutoMapper Magic (Chapter 06)**
```csharp
[HttpPost]
public ActionResult<ProductResponseDto> CreateProduct(CreateProductDto createDto)
{
    // ✅ Clean, concise mapping
    var product = mapper.Map<Product>(createDto);
    product.Id = products.Max(x => x.Id) + 1;
    
    products.Add(product);
    
    return Ok(mapper.Map<ProductResponseDto>(product));
}
```

## 🎛️ Advanced Mapping Scenarios

### **Conditional Mapping (PATCH operations)**
```csharp
CreateMap<PatchProductDto, Product>()
    .ForAllMembers(opt => opt.Condition((src, dest, member) => member != null));
```

### **Custom Value Resolvers**
```csharp
CreateMap<CreateProductDto, Product>()
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
    .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));
```

### **Ignoring Properties**
```csharp
CreateMap<CreateProductDto, Product>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.InternalNotes, opt => opt.Ignore());
```

## 🧪 Testing the Enhanced API

### **Create Product Request**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Mechanical Gaming Keyboard",
  "price": 149.99,
  "category": "Gaming"
}
```

### **Response (Automatically Mapped)**
```json
{
  "id": 5,
  "name": "Mechanical Gaming Keyboard",
  "price": 149.99,
  "category": "Gaming",
  "createdAt": "2025-08-06T14:30:00Z"
}
```

## 🎓 AutoMapper Benefits

### **1. Reduced Boilerplate**
- **Before**: 20+ lines of manual mapping per method
- **After**: 1-2 lines with `mapper.Map<T>()`

### **2. Maintainability**
- **Centralized mapping logic** in profiles
- **Easy to update** when models change
- **Consistent mapping** across the application

### **3. Performance**
- **Compiled mappings** for better performance
- **Cached expressions** reduce reflection overhead
- **Memory efficient** object creation

### **4. Advanced Features**
- **Conditional mapping** for complex scenarios
- **Custom value resolvers** for calculated fields
- **Projection support** for Entity Framework queries

## 🔧 Running the Project

```bash
cd 06-automapper/AutoMapper
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`

## 🏗️ Architecture Benefits

### **Clean Controller Actions**
```csharp
public class ProductsController : ControllerBase
{
    private readonly IMapper mapper;
    
    public ProductsController(IMapper mapper)
    {
        this.mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<ProductResponseDto>> GetProducts()
    {
        return Ok(mapper.Map<IEnumerable<ProductResponseDto>>(products));
    }
}
```

### **Separation of Concerns**
- **Controllers** focus on HTTP concerns
- **Mapping Profiles** handle object transformation
- **Models** represent business entities
- **DTOs** define API contracts

## 📈 Performance Considerations

### **AutoMapper Best Practices**
1. **Register once** in dependency injection
2. **Use profiles** to organize mapping logic
3. **Avoid complex expressions** in mapping configuration
4. **Profile compilation** happens at startup for better runtime performance

### **Memory Usage**
```csharp
// ✅ Good - single mapping operation
var responseDto = mapper.Map<ProductResponseDto>(product);

// ❌ Avoid - multiple small mappings in loops
foreach(var product in products)
{
    var dto = mapper.Map<ProductResponseDto>(product); // Better to map collection at once
}

// ✅ Better - map entire collection
var responseDtos = mapper.Map<IEnumerable<ProductResponseDto>>(products);
```

## 🔍 Debugging AutoMapper

### **Configuration Validation**
```csharp
// In Program.cs - validate mappings at startup
var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();
```

### **Common Mapping Issues**
- **Missing mappings** - Add to profile
- **Property name mismatches** - Use ForMember()
- **Type conversion errors** - Add custom converters

## 🎯 Key Takeaways

1. **AutoMapper eliminates repetitive mapping code**
2. **Mapping profiles centralize transformation logic**
3. **Performance is excellent when configured correctly**
4. **DTOs + AutoMapper = Clean, maintainable APIs**

## ➡️ What's Next?

🎉 **Congratulations!** You've completed the core ASP.NET Core 9 Web API tutorial series!

**Next learning paths:**
- **Entity Framework Core** for database integration
- **Authentication & Authorization** for secure APIs
- **Caching strategies** for performance optimization
- **API versioning** for evolving APIs
- **Unit testing** for reliable code

## 🤔 Troubleshooting

**AutoMapper not working?**
- Check DI registration in Program.cs
- Verify mapping profile is included in AddAutoMapper()

**Mapping exceptions?**
- Use `AssertConfigurationIsValid()` to catch issues early
- Check property names and types match

**Performance concerns?**
- Profile mapping operations in development
- Consider ProjectTo() for Entity Framework queries

---

**💡 Pro Tip**: AutoMapper shines in larger applications - the time saved on mapping code adds up quickly!
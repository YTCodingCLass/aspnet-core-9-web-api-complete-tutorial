# نمط Repository والخدمات مع حقن التبعية

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Repository Pattern](https://img.shields.io/badge/Repository_Pattern-Architecture-FF6B35?style=flat-square)
![Service Layer](https://img.shields.io/badge/Service_Layer-Business_Logic-2E8B57?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد درس نمط Repository والخدمات](https://www.youtube.com/watch?v=ZoW5RnvgeSo&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=6)**

## 🎯 أهداف التعلم

في نهاية هذا الدرس، ستتقن:
- ✅ **نمط Repository** - تجريد طبقة الوصول للبيانات عن منطق الأعمال
- ✅ **معمارية طبقة الخدمات** - تنفيذ منطق الأعمال وتنسيق الخدمات
- ✅ **حقن التبعية** - إدارة تبعيات المكونات ودورات حياتها
- ✅ **نمط DTO** - فصل النماذج الداخلية عن عقود API
- ✅ **تكامل AutoMapper** - تحويل تلقائي من كائن إلى كائن
- ✅ **المعمارية النظيفة** - بناء APIs قابلة للصيانة والاختبار والتوسع

## 🚀 ما سنبنيه

**API إدارة المنتجات** يوضح:
- **نمط Repository** - تجريد نظيف للوصول للبيانات
- **طبقة الخدمات** - منطق الأعمال والتحقق من صحة البيانات
- **حقن التبعية** - ربط مناسب للمكونات
- **تكامل AutoMapper** - تحويل تلقائي للـ DTOs
- **خدمة الإشعارات** - الاهتمامات الشاملة

## 📁 هيكل المشروع

```
RepositoryAndServicesApi/
├── Controllers/
│   ├── ProductsController.cs        # متحكم API المنتجات
│   └── ServiceLifetimeController.cs # عرض دورات حياة الخدمات
├── Models/
│   ├── Product.cs                   # نموذج كيان المنتج
│   ├── Supplier.cs                  # نموذج كيان المورد
│   └── DTOs/
│       ├── CreateProductDto.cs      # DTO طلب إنشاء منتج
│       ├── UpdateProductDto.cs      # DTO طلب تحديث منتج
│       ├── PatchProductDto.cs       # DTO طلب تعديل جزئي للمنتج
│       └── ProductResponseDto.cs    # DTO استجابة المنتج مع حالة المخزون
├── Repositories/
│   ├── IProductRepository.cs        # واجهة المستودع للوصول للبيانات
│   └── ProductRepository.cs         # تنفيذ المستودع
├── Services/
│   ├── IProductService.cs           # واجهة الخدمة لمنطق الأعمال
│   ├── ProductService.cs            # تنفيذ الخدمة
│   ├── INotificationService.cs      # واجهة خدمة الإشعارات
│   └── NotificationService.cs       # تنفيذ خدمة الإشعارات
├── Data/
│   └── InMemoryDatabase.cs          # قاعدة بيانات في الذاكرة
├── Services/
│   └── BusinessException.cs         # استثناءات منطق الأعمال (نُقلت لمجلد Services)
├── Mappings/
│   └── MappingProfile.cs            # تكوين AutoMapper
├── Program.cs                       # تسجيل حاوي DI وتكوين التطبيق
└── RepositoryAndServicesApi.http    # طلبات HTTP للاختبار
```

## 🏗️ نظرة عامة على المعمارية

تتبع المعمارية نهجاً طبقياً نظيفاً كما هو موضح في المخطط:

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

### **مسؤوليات الطبقات:**
- **REST Controller**: نقاط النهاية HTTP، معالجة الطلبات/الاستجابات، التحقق من الصحة
- **Services**: منطق الأعمال، تحويل البيانات، الاهتمامات الشاملة
- **Mappers**: تحويل الكائنات بين DTOs والكيانات باستخدام AutoMapper
- **Repository**: تجريد الوصول للبيانات، عمليات CRUD
- **DTOs**: كائنات نقل البيانات لعقود API
- **Entities**: نماذج النطاق التي تمثل بيانات الأعمال

### **1. تنفيذ نمط Repository**
```csharp
// Repositories/IProductRepository.cs - تجريد الوصول للبيانات
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

// Repositories/ProductRepository.cs - تنفيذ الوصول للبيانات
public class ProductRepository : IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ILogger<ProductRepository> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        _logger.LogInformation("🗃️ Repository: الحصول على جميع المنتجات");
        
        return InMemoryDatabase.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToList();
    }
    
    // باقي طرق المستودع...
}
```

### **2. تنفيذ طبقة الخدمات**
```csharp
// Services/IProductService.cs - واجهة منطق الأعمال
public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<ProductResponseDto?> GetProductByIdAsync(int id);
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto);
    Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
    Task<bool> DeleteProductAsync(int id);
}

// Services/ProductService.cs - تنفيذ منطق الأعمال
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
        _logger.LogInformation("⚙️ Service: الحصول على جميع المنتجات");
        
        var products = await _productRepository.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        
        // منطق الأعمال: حساب حالة المخزون
        foreach (var dto in productDtos)
        {
            dto.StockStatus = CalculateStockStatus(dto.StockQuantity);
        }
        
        return productDtos;
    }
    
    // طرق منطق الأعمال...
    private string CalculateStockStatus(int stockQuantity)
    {
        return stockQuantity switch
        {
            0 => "نفد المخزون",
            <= 5 => "منخفض جداً",
            <= 10 => "منخفض",
            <= 50 => "طبيعي",
            _ => "متوفر جيداً"
        };
    }
}
```

### **3. تسجيل حقن التبعية**
```csharp
// Program.cs - تسجيل الخدمات
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

## 🎮 تنفيذ الكونترولر

### **كونترولر المنتجات بالمعمارية النظيفة**
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
        _logger.LogInformation("🎮 Controller: الحصول على جميع المنتجات");
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
    {
        _logger.LogInformation("🎮 Controller: الحصول على المنتج {ProductId}", id);
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null)
        {
            return NotFound($"المنتج برقم {id} غير موجود");
        }
        
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct(CreateProductDto createProductDto)
    {
        _logger.LogInformation("🎮 Controller: إنشاء المنتج {ProductName}", createProductDto.Name);
        
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
        _logger.LogInformation("🎮 Controller: تحديث المنتج {ProductId}", id);
        
        try
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);
            if (updatedProduct == null)
            {
                return NotFound($"المنتج برقم {id} غير موجود");
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
        _logger.LogInformation("🎮 Controller: حذف المنتج {ProductId}", id);
        
        try
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound($"المنتج برقم {id} غير موجود");
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

## 🏛️ فوائد المعمارية

### **🗃️ فوائد نمط Repository**
- **تجريد الوصول للبيانات** - إخفاء تفاصيل تنفيذ تخزين البيانات
- **قابلية الاختبار** - سهولة إنشاء Mock للوصول للبيانات في اختبارات الوحدة
- **المرونة** - التبديل بين مصادر بيانات مختلفة دون تغيير منطق الأعمال
- **المسؤولية الواحدة** - المستودعات تتعامل فقط مع عمليات الوصول للبيانات

```csharp
// سهولة التبديل من الذاكرة إلى قاعدة البيانات
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // في الذاكرة
// builder.Services.AddScoped<IProductRepository, SqlProductRepository>(); // SQL Server
// builder.Services.AddScoped<IProductRepository, MongoProductRepository>(); // MongoDB
```

### **⚙️ فوائد طبقة الخدمات**
- **تغليف منطق الأعمال** - جميع قواعد الأعمال في مكان واحد
- **الاهتمامات الشاملة** - التعامل مع الإشعارات، التحقق، التسجيل
- **تحويل DTO** - التحويل بين نماذج النطاق و DTOs
- **إدارة المعاملات** - تنسيق عمليات متعددة من المستودعات

```csharp
// الخدمة تنسق عمليات متعددة
public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto)
{
    // 1. التحقق من قواعد الأعمال
    await ValidateProductCreationAsync(createDto);
    
    // 2. تحويل DTO إلى نموذج النطاق
    var product = _mapper.Map<Product>(createDto);
    
    // 3. الحفظ في المستودع
    var createdProduct = await _productRepository.CreateAsync(product);
    
    // 4. إرسال الإشعارات
    await _notificationService.SendProductCreatedNotificationAsync(responseDto);
    
    // 5. إرجاع DTO الاستجابة
    return _mapper.Map<ProductResponseDto>(createdProduct);
}
```

### **🔗 فوائد حقن التبعية**
- **الربط المرن** - المكونات تعتمد على التجريدات، وليس التنفيذات
- **عكس التحكم** - الإطار يدير دورة حياة الكائنات والتبعيات
- **مرونة التكوين** - سهولة تغيير التنفيذات عبر التسجيل
- **إدارة دورة الحياة المحدودة** - إدارة مناسبة للموارد والتخلص منها

```csharp
// فصل نظيف للاهتمامات عبر DI
builder.Services.AddScoped<IProductRepository, ProductRepository>();    // الوصول للبيانات
builder.Services.AddScoped<IProductService, ProductService>();          // منطق الأعمال
builder.Services.AddScoped<INotificationService, NotificationService>(); // الاهتمامات الشاملة
builder.Services.AddAutoMapper(typeof(MappingProfile));                 // تحويل الكائنات
```

## 🧪 اختبار API

### **عمليات CRUD للمنتجات**

#### **1. الحصول على جميع المنتجات**
```http
GET https://localhost:7xxx/api/products
```

#### **2. الحصول على منتج بالمعرف**
```http
GET https://localhost:7xxx/api/products/1
```

#### **3. إنشاء منتج جديد**
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "سماعات لاسلكية",
  "price": 99.99,
  "costPrice": 50.00,
  "category": "إلكترونيات",
  "stockQuantity": 25,
  "supplierId": 1
}
```

#### **4. تحديث منتج موجود**
```http
PUT https://localhost:7xxx/api/products/1
Content-Type: application/json

{
  "name": "لابتوب محدث برو",
  "price": 1299.99,
  "costPrice": 800.00,
  "category": "إلكترونيات",
  "stockQuantity": 15,
  "supplierId": 2
}
```

#### **5. حذف منتج**
```http
DELETE https://localhost:7xxx/api/products/1
```

### **نموذج استجابة API**
```json
{
  "id": 1,
  "name": "لابتوب برو",
  "price": 1199.99,
  "category": "إلكترونيات",
  "stockQuantity": 10,
  "stockStatus": "منخفض",
  "supplier": {
    "id": 1,
    "name": "شركة الحلول التقنية",
    "email": "contact@techsolutions.com"
  },
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T14:45:00Z"
}
```

### **مخرجات التسجيل في وحدة التحكم**
```
🗃️ Repository: الحصول على جميع المنتجات
⚙️ Service: الحصول على جميع المنتجات
⚙️ Service: تم استرداد 5 منتجات
🎮 Controller: الحصول على جميع المنتجات

🗃️ Repository: إنشاء منتج جديد: سماعات لاسلكية  
⚙️ Service: إنشاء منتج جديد: سماعات لاسلكية
📧 Notification: تم إنشاء المنتج - سماعات لاسلكية
🎮 Controller: إنشاء المنتج سماعات لاسلكية
```

## 🎓 أنماط المعمارية الرئيسية

### **1. تنفيذ نمط Repository**
- ✅ **تجريد الوصول للبيانات** - `IProductRepository` يخفي تفاصيل تخزين البيانات
- ✅ **العمليات غير المتزامنة** - جميع طرق المستودع غير متزامنة
- ✅ **علاقات الكيانات** - `GetByIdWithSupplierAsync` يحمل الكيانات المرتبطة
- ✅ **دعم التصفية** - طرق للتصفية حسب الفئة والمخزون المنخفض

### **2. تنفيذ طبقة الخدمات**
- ✅ **تغليف منطق الأعمال** - حساب حالة المخزون، قواعد التحقق
- ✅ **تحويل DTO** - التحويل التلقائي بين الكيانات و DTOs
- ✅ **الاهتمامات الشاملة** - الإشعارات، التسجيل، معالجة الاستثناءات
- ✅ **التحقق من الصحة** - تطبيق قواعد الأعمال (هوامش الربح، الأسماء المكررة)

### **3. فوائد حقن التبعية**
- ✅ **الربط المرن** - سلسلة Controller → Service → Repository
- ✅ **قابلية الاختبار** - سهولة إنشاء mock للتبعيات لاختبار الوحدة
- ✅ **المسؤولية الواحدة** - كل مكون له غرض واضح واحد
- ✅ **مرونة التكوين** - سهولة تبديل التنفيذات

### **4. تدفق المعمارية النظيفة**
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   DTOs      │◄──►│ Controller  │────│   Service   │────│ Repository  │
│             │    │             │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                           │                   │                   │
                           ▼                   ▼                   ▼
                   معالجة HTTP         منطق الأعمال        الوصول للبيانات
                   التحقق من الإدخال   تحويل البيانات      عمليات الاستعلام
                   استجابات الأخطاء    الاهتمامات الشاملة  تحويل الكيانات
                                     تنسيق الخدمات
                                           │
                                           ▼
                                   ┌─────────────┐    ┌─────────────┐
                                   │   Mappers   │◄──►│  Entities   │
                                   │(AutoMapper) │    │ (Models)    │
                                   └─────────────┘    └─────────────┘
```

## 🔧 تشغيل المشروع

```bash
cd RepositoryAndServicesApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`
**عرض الخدمة**: `https://localhost:7xxx/api/servicelifetime/demo`

## 🏗️ أفضل الممارسات في التنفيذ

### **إرشادات نمط Repository**
```csharp
// ✅ افعل: اجعل المستودعات تركز على الوصول للبيانات
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();           // عمليات الاستعلام
    Task<Product> CreateAsync(Product product);         // عمليات الأمر
    Task<bool> ExistsAsync(int id);                    // عمليات المساعدة
}

// ❌ لا تفعل: لا تضع منطق الأعمال في المستودعات
public interface IProductRepository
{
    Task<decimal> CalculateTotalValue();  // منطق الأعمال ينتمي للخدمة
    Task SendLowStockAlert();            // الاهتمام الشامل ينتمي للخدمة
}
```

### **إرشادات طبقة الخدمات**
```csharp
// ✅ افعل: غلف منطق الأعمال في الخدمات
public class ProductService : IProductService
{
    // التحقق من منطق الأعمال
    private async Task ValidateProductCreationAsync(CreateProductDto dto) { }
    
    // حسابات منطق الأعمال
    private string CalculateStockStatus(int stock) { }
    
    // تنسيق عمليات متعددة
    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto) { }
}

// ❌ لا تفعل: لا تضع منطق الوصول للبيانات في الخدمات
public class ProductService : IProductService
{
    public async Task<Product> GetProductAsync(int id)
    {
        // لا تكتب SQL أو كود الوصول للبيانات هنا
        var sql = "SELECT * FROM Products WHERE Id = @id";
        return await _connection.QueryAsync<Product>(sql, new { id });
    }
}
```

### **⚠️ الأنماط المضادة الشائعة**
- **لا تضع** منطق الأعمال في الكونترولرز - استخدم الخدمات
- **لا تصل** للمستودعات مباشرة من الكونترولرز - استخدم الخدمات
- **لا ترجع** كيانات النطاق من الكونترولرز - استخدم DTOs
- **لا تلتقط** الاستثناءات في المستودعات - دع الخدمات تتعامل معها

## 📈 عرض تأثير الأداء

### **التجربة: طلبات متعددة**
1. **الطلب الأول** - جميع الخدمات يتم إنشاؤها:
```
🔴 تم إنشاء Singleton: [GUID-1]
🟡 تم إنشاء Scoped: [GUID-2] 
🟢 تم إنشاء Transient: [GUID-3]
🟢 تم إنشاء Transient: [GUID-4]
```

2. **الطلب الثاني** - فقط Scoped و Transient يتم إنشاؤهما:
```
🟡 تم إنشاء Scoped: [GUID-5]  // جديد لهذا الطلب
🟢 تم إنشاء Transient: [GUID-6]  // نسخ جديدة
🟢 تم إنشاء Transient: [GUID-7]
```

3. **الطلب الثالث** - النمط يستمر:
```
🟡 تم إنشاء Scoped: [GUID-8]  // جديد لهذا الطلب
🟢 تم إنشاء Transient: [GUID-9]  // دائماً جديد
🟢 تم إنشاء Transient: [GUID-10]
```

### **تأثير الذاكرة والـ CPU**
- **Singleton**: صفر تخصيص بعد الطلب الأول
- **Scoped**: تخصيص واحد لكل طلب
- **Transient**: تخصيصات متعددة لكل طلب

## 🔍 تصحيح دورات حياة الخدمات

### **المشاكل الشائعة والحلول**

**1. مشكلة التبعية الأسيرة**
```csharp
// ❌ سيء: Singleton يحبس خدمة Scoped
public class MySingleton
{
    private readonly DbContext _context; // Scoped!
    // هذا سيسبب مشاكل!
}

// ✅ جيد: استخدم IServiceProvider أو نمط factory
public class MySingleton
{
    private readonly IServiceProvider _serviceProvider;
    
    public void DoSomething()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        // استخدم السياق بأمان
    }
}
```

**2. تصحيح إنشاء الخدمة**
```csharp
// أضف تسجيل للبنائيات
public MyService(ILogger<MyService> logger)
{
    logger.LogInformation("تم إنشاء MyService: {InstanceId}", Guid.NewGuid());
}
```

## 🎯 النقاط الرئيسية

1. **نمط Repository**: يجرد الوصول للبيانات ويسهل الاختبار
2. **طبقة الخدمات**: تغلف منطق الأعمال وتنسق العمليات  
3. **حقن التبعية**: يمكن الربط المرن والتكوين المرن
4. **نمط DTO**: يفصل النماذج الداخلية عن عقود API
5. **المعمارية النظيفة**: كل طبقة لها مسؤولية واحدة وحدود واضحة

## ➡️ ماذا بعد؟

**توسيع هذه المعمارية مع:**
- **Entity Framework Core** - استبدال البيانات في الذاكرة بقاعدة بيانات حقيقية
- **نمط Unit of Work** - إدارة المعاملات عبر مستودعات متعددة
- **نمط CQRS** - فصل عمليات القراءة والكتابة
- **MediatR** - تنفيذ نمط الطلب/الاستجابة مع المعالجات
- **الخدمات الخلفية** - إضافة معالجة غير متزامنة للإشعارات

## 🤔 استكشاف الأخطاء وإصلاحها

**مشاكل تسجيل الخدمات؟**
- تحقق من تسجيل جميع الواجهات والتنفيذات في `Program.cs`
- تأكد من توافق دورات حياة الخدمات (لا تحقن scoped في singleton)

**مشاكل تكوين AutoMapper؟**
- تأكد من تسجيل `MappingProfile`: `builder.Services.AddAutoMapper(typeof(MappingProfile))`
- تحقق من تكوين جميع تحويلات DTO-Entity بشكل صحيح

**التحقق من منطق الأعمال لا يعمل؟**
- تحقق من أن `BusinessException` يتم رميها بشكل صحيح من طبقة الخدمة
- تأكد من أن الكونترولر يلتقط `BusinessException` ويرجع استجابة HTTP مناسبة

**المستودع يرجع null؟**
- تحقق من وجود الكيان وأن علامة `IsActive` true (تنفيذ الحذف الناعم)
- تأكد من أن الطرق غير المتزامنة يتم انتظارها بشكل صحيح

---

**💡 نصيحة احترافية**: ابدأ بهذا الأساس للمعمارية النظيفة وأضف التعقيد تدريجياً مع نمو تطبيقك!
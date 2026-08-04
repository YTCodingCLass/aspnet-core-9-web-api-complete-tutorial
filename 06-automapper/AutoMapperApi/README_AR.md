# الفصل 06: تكامل AutoMapper - التخلص من الكود المكرر

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![AutoMapper](https://img.shields.io/badge/AutoMapper-Mapping-FF6B35?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد الفصل 06: تكامل AutoMapper](https://www.youtube.com/watch?v=JlJGgamL5Iw&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=8)**

## 🎯 أهداف التعلم

في نهاية هذا الفصل، ستتقن:
- ✅ تثبيت وتكوين AutoMapper في ASP.NET Core
- ✅ إنشاء ملفات تعريف الربط للـ DTOs والكيانات
- ✅ التخلص من كود الربط اليدوي المكرر
- ✅ السيناريوهات المتقدمة وتكوينات مخصصة
- ✅ أفضل الممارسات لربط الكائنات في Web APIs

## 🚀 ما سنبنيه

API منتجات محسن مع AutoMapper:
- **ربط تلقائي** بين DTOs والكيانات
- **حساب حالة المخزون** مع منطق ربط مخصص  
- **معالجة معلومات المورد** في الاستجابات
- **ملفات تعريف الربط** للتكوين المنظم
- **كونترولرز أنظف** مع كود مكرر أقل

## 📁 هيكل المشروع

```
AutoMapperApi/
├── Controllers/
│   └── ProductsController.cs    # كونترولرز نظيفة مع AutoMapper
├── Models/
│   ├── Product.cs              # نموذج الكيان مع المورد
│   └── DTOs/
│       ├── CreateProductDto.cs     # DTOs الطلبات
│       ├── UpdateProductDto.cs     
│       ├── PatchProductDto.cs      
│       └── ProductResponseDto.cs   # DTO الاستجابة مع حالة المخزون
├── Mappings/
│   └── MappingProfile.cs       # تكوين AutoMapper
├── Program.cs                  # تسجيل AutoMapper DI
└── AutoMapperApi.http          # طلبات HTTP
```

## 🔧 إعداد AutoMapper

### **1. تثبيت حزمة AutoMapper**
```bash
dotnet add package AutoMapper
```

### **2. إنشاء ملف تعريف الربط**
```csharp
// Mappings/MappingProfile.cs
using AutoMapper;
using AutoMapperApi.Models;
using AutoMapperApi.Models.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // الكيان إلى DTO الاستجابة مع حساب حالة المخزون
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.StockStatus,
                opt => opt.MapFrom(src => GetStockStatus(src.StockQuantity)));

        // DTO الإنشاء إلى الكيان
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, 
                opt => opt.Ignore());

        // DTO التحديث إلى الكيان
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Id, 
                opt => opt.Ignore());
    }

    private static string GetStockStatus(int stockQuantity)
    {
        return stockQuantity switch
        {
            0 => "نفد من المخزون",
            <= 10 => "مخزون منخفض",
            _ => "متوفر"
        };
    }
}
```

### **3. تسجيل AutoMapper في حاوي DI**
```csharp
// Program.cs
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

## 💻 قبل وبعد AutoMapper

### **قبل - الربط اليدوي (الفصل 05)**
```csharp
[HttpPost]
public ActionResult<ProductResponseDto> CreateProduct(CreateProductDto createDto)
{
    // ❌ ربط يدوي - مكرر وعرضة للأخطاء
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
    
    // ربط الاستجابة اليدوي
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

### **بعد - سحر AutoMapper (الفصل 06)**
```csharp
[HttpPost]
public ActionResult<ProductResponseDto> CreateProduct(CreateProductDto createDto)
{
    // ✅ ربط نظيف ومختصر
    var product = mapper.Map<Product>(createDto);
    product.Id = products.Max(x => x.Id) + 1;
    
    products.Add(product);
    
    return Ok(mapper.Map<ProductResponseDto>(product));
}
```

## 🎛️ السيناريوهات المتقدمة للربط

### **حساب حالة المخزون**
```csharp
CreateMap<Product, ProductResponseDto>()
    .ForMember(dest => dest.StockStatus,
        opt => opt.MapFrom(src => GetStockStatus(src.StockQuantity)));

private static string GetStockStatus(int stockQuantity)
{
    return stockQuantity switch
    {
        0 => "نفد من المخزون",
        <= 10 => "مخزون منخفض",
        _ => "متوفر"
    };
}
```

### **حلالات القيم المخصصة**
```csharp
CreateMap<CreateProductDto, Product>()
    .ForMember(dest => dest.Id, opt => opt.Ignore());
```

### **تجاهل الخصائص**
```csharp
CreateMap<CreateProductDto, Product>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.InternalNotes, opt => opt.Ignore());
```

## 🧪 اختبار API المحسن

### **طلب إنشاء منتج**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Mechanical Gaming Keyboard",
  "price": 149.99,
  "category": "Gaming",
  "stockQuantity": 25
}
```

### **الاستجابة (مربوطة تلقائياً)**
```json
{
  "id": 5,
  "name": "Mechanical Gaming Keyboard",
  "price": 149.99,
  "category": "Gaming",
  "createdAt": "2025-08-10T14:30:00Z",
  "stockQuantity": 25,
  "stockStatus": "متوفر",
  "supplierCompanyName": null,
  "supplierContactName": null
}
```

## 🎓 فوائد AutoMapper

### **1. تقليل الكود المكرر**
- **قبل**: 20+ سطر من الربط اليدوي لكل طريقة
- **بعد**: 1-2 سطر مع `mapper.Map<T>()`

### **2. القابلية للصيانة**
- **منطق ربط مركزي** في الملفات الشخصية
- **سهولة التحديث** عند تغيير النماذج
- **ربط ثابت** عبر التطبيق

### **3. الأداء**
- **ربط مجمع** لأداء أفضل
- **تعبيرات مخزنة مؤقتاً** تقلل من overhead الانعكاس
- **إنشاء كائنات فعال في الذاكرة**

### **4. الميزات المتقدمة**
- **ربط شرطي** للسيناريوهات المعقدة
- **حلالات القيم المخصصة** للحقول المحسوبة
- **دعم الإسقاط** لاستعلامات Entity Framework

## 🔧 تشغيل المشروع

```bash
cd 06-automapper/AutoMapperApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`

## 🏗️ فوائد المعمارية

### **إجراءات كونترولر نظيفة**
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

### **فصل الاهتمامات**
- **الكونترولرز** تركز على اهتمامات HTTP
- **ملفات الربط** تتعامل مع تحويل الكائنات
- **النماذج** تمثل كيانات العمل
- **DTOs** تحدد عقود API

## 📈 اعتبارات الأداء

### **أفضل ممارسات AutoMapper**
1. **التسجيل مرة واحدة** في حقن التبعية
2. **استخدام الملفات الشخصية** لتنظيم منطق الربط
3. **تجنب التعبيرات المعقدة** في تكوين الربط
4. **تجميع الملفات الشخصية** يحدث عند بدء التشغيل لأداء runtime أفضل

### **استخدام الذاكرة**
```csharp
// ✅ جيد - عملية ربط واحدة
var responseDto = mapper.Map<ProductResponseDto>(product);

// ❌ تجنب - عدة ربطات صغيرة في الحلقات
foreach(var product in products)
{
    var dto = mapper.Map<ProductResponseDto>(product); // أفضل ربط المجموعة مرة واحدة
}

// ✅ أفضل - ربط المجموعة كاملة
var responseDtos = mapper.Map<IEnumerable<ProductResponseDto>>(products);
```

## 🔍 تصحيح AutoMapper

### **التحقق من التكوين**
```csharp
// في Program.cs - التحقق من الربطات عند البدء
var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();
```

### **مشاكل الربط الشائعة**
- **ربطات مفقودة** - أضف إلى الملف الشخصي
- **عدم تطابق أسماء الخصائص** - استخدم ForMember()
- **أخطاء تحويل النوع** - أضف محولات مخصصة

## 🎯 النقاط الرئيسية

1. **AutoMapper يزيل كود الربط المكرر**
2. **ملفات الربط تركز منطق التحويل**
3. **الأداء ممتاز عند التكوين الصحيح**
4. **DTOs + AutoMapper = APIs نظيفة وقابلة للصيانة**

## ➡️ ماذا بعد؟

🎉 **تهانينا!** لقد أكملت سلسلة دروس ASP.NET Core 9 Web API الأساسية!

**مسارات التعلم التالية:**
- **Entity Framework Core** لتكامل قاعدة البيانات
- **المصادقة والتفويض** لـ APIs آمنة
- **استراتيجيات التخزين المؤقت** لتحسين الأداء
- **إصدار API** لـ APIs متطورة
- **اختبار الوحدة** لكود موثوق

## 🤔 استكشاف الأخطاء وإصلاحها

**AutoMapper لا يعمل؟**
- تحقق من تسجيل DI في Program.cs
- تحقق من تضمين الملف الشخصي للربط في AddAutoMapper()

**استثناءات الربط؟**
- استخدم `AssertConfigurationIsValid()` لاكتشاف المشاكل مبكراً
- تحقق من تطابق أسماء وأنواع الخصائص

**مخاوف الأداء؟**
- قم بتحليل عمليات الربط في التطوير
- فكر في ProjectTo() لاستعلامات Entity Framework

---

**💡 نصيحة احترافية**: AutoMapper يتألق في التطبيقات الكبيرة - الوقت المحفوظ في كود الربط يتراكم بسرعة!
# الفصل 05: DTOs والتحقق - تصميم API احترافي

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Validation](https://img.shields.io/badge/Validation-DataAnnotations-4CAF50?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد الفصل 05: DTOs والتحقق](https://www.youtube.com/watch?v=zwp3Qvxxzgc&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=5)**

## 🎯 أهداف التعلم

في نهاية هذا الفصل، ستتقن:
- ✅ نمط كائنات نقل البيانات (DTOs)
- ✅ التحقق من الإدخال باستخدام DataAnnotations
- ✅ فصل النماذج الداخلية عن عقود API
- ✅ رسائل التحقق المخصصة ومعالجة الأخطاء
- ✅ أنماط ربط الاستجابات

## 🚀 ما سنبنيه

API منتجات احترافي مع:
- **DTOs منفصلة** للطلبات والاستجابات
- **تحقق شامل** مع رسائل خطأ مخصصة
- **فصل نظيف** بين النماذج الداخلية وعقود API
- **استجابات أخطاء صحيحة** مع تفاصيل التحقق

## 📁 هيكل المشروع

```
DtoAndValidation/
├── Controllers/
│   └── ProductsController.cs    # كونترولر محدث مع DTOs
├── Models/
│   ├── Product.cs              # نموذج الكيان الداخلي
│   ├── CreateProductDto.cs     # DTO لإنشاء المنتجات
│   ├── UpdateProductDto.cs     # DTO لتحديث المنتجات
│   ├── PatchProductDto.cs      # DTO للتحديثات الجزئية
│   └── ProductResponseDto.cs   # DTO لاستجابات API
├── Program.cs                  # تكوين التحقق
└── DtoAndValidation.http       # طلبات HTTP مع DTOs
```

## 🔧 تنفيذ نمط DTO

### **لماذا DTOs مهمة**
```csharp
// ❌ لا تعرض النماذج الداخلية مباشرة
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string InternalNotes { get; set; }    // ⚠️ داخلي فقط!
    public bool IsDeleted { get; set; }          // ⚠️ داخلي فقط!
    public int CreatedByUserId { get; set; }     // ⚠️ داخلي فقط!
    public decimal CostPrice { get; set; }       // ⚠️ داخلي فقط!
}

// ✅ استخدم DTOs لعقود API نظيفة
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

### **DTOs الطلبات**

**CreateProductDto.cs**
```csharp
public class CreateProductDto
{
    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    [StringLength(100, MinimumLength = 2, 
        ErrorMessage = "اسم المنتج يجب أن يكون بين 2 و 100 حرف")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(0.01, double.MaxValue, 
        ErrorMessage = "السعر يجب أن يكون أكبر من 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "الفئة مطلوبة")]
    [StringLength(50, MinimumLength = 2, 
        ErrorMessage = "الفئة يجب أن تكون بين 2 و 50 حرف")]
    public string Category { get; set; } = string.Empty;
}
```

### **DTO الاستجابة**

**ProductResponseDto.cs**
```csharp
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    // ملاحظة: لا حقول داخلية معروضة!
}
```

## 🛡️ التحقق في العمل

### **خصائص DataAnnotations**
- `[Required]` - الحقل يجب تقديمه
- `[StringLength(max, MinimumLength = min)]` - قيود طول النص
- `[Range(min, max)]` - التحقق من النطاق الرقمي
- `[EmailAddress]` - التحقق من تنسيق البريد الإلكتروني
- `[RegularExpression]` - مطابقة الأنماط المخصصة

### **كونترولر مع ربط DTO**
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

## 🧪 اختبار التحقق

### **طلب صحيح**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Gaming Keyboard",
  "price": 129.99,
  "category": "Gaming"
}
```

### **طلب غير صحيح (يؤدي إلى التحقق)**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "",           // ❌ حقل مطلوب فارغ
  "price": -10,         // ❌ سعر سالب
  "category": "A"       // ❌ قصير جداً (أدنى 2 أحرف)
}
```

### **استجابة خطأ التحقق**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["اسم المنتج مطلوب"],
    "Price": ["السعر يجب أن يكون أكبر من 0"],
    "Category": ["الفئة يجب أن تكون بين 2 و 50 حرف"]
  }
}
```

## 🎭 أنماط ربط DTO

### **الربط اليدوي (التنفيذ الحالي)**
```csharp
// في Product.cs
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

### **فوائد هذا النمط**
1. **الأمان** - الحقول الداخلية لا تُعرض أبداً
2. **المرونة** - API يمكن أن يتطور بشكل مستقل عن قاعدة البيانات
3. **التحقق** - تحقق قوي من الإدخال عند حدود API
4. **التوثيق** - عقود واضحة لمستهلكي API

## 🔍 شرح أنواع DTO

### **CreateProductDto** 
- مستخدم لطلبات **POST**
- يحتوي فقط على الحقول المطلوبة للإنشاء
- تحقق قوي لسلامة البيانات

### **UpdateProductDto**
- مستخدم لطلبات **PUT** (التحديثات الكاملة)
- يحتوي على جميع الحقول القابلة للتحديث
- التحقق يضمن استبدال المورد كاملاً

### **PatchProductDto**
- مستخدم لطلبات **PATCH** (التحديثات الجزئية)
- جميع الحقول اختيارية للمرونة
- التحقق الشرطي بناءً على الحقول المقدمة

### **ProductResponseDto**
- مستخدم لجميع عمليات **الاستجابة**
- يحتوي فقط على الحقول المواجهة للعامة
- تنسيق استجابة ثابت عبر النقاط النهائية

## 📊 مقارنة قبل وبعد

### **قبل DTOs (الفصل 04)**
```csharp
// ❌ عرض النموذج الداخلي
[HttpPost]
public ActionResult<Product> CreateProduct(Product product)
{
    // يمكن للعملاء تعيين الحقول الداخلية!
    // لا حدود للتحقق
}
```

### **بعد DTOs (الفصل 05)**
```csharp
// ✅ نهج نظيف قائم على DTO
[HttpPost]
public ActionResult<ProductResponseDto> CreateProduct(CreateProductDto createDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
        
    // ربط DTO بالنموذج الداخلي
    // عرض الحقول العامة فقط في الاستجابة
}
```

## 🧪 اختبار API الكامل

### **إنشاء منتج مع التحقق**
```http
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Wireless Gaming Mouse",
  "price": 89.99,
  "category": "Gaming Accessories"
}
```

### **تحديث المنتج (PUT)**
```http
PUT https://localhost:7185/api/products/1
Content-Type: application/json

{
  "name": "Updated Gaming Mouse",
  "price": 99.99,
  "category": "Gaming"
}
```

### **التحديث الجزئي (PATCH)**
```http
PATCH https://localhost:7185/api/products/1
Content-Type: application/json

{
  "price": 79.99
}
```

## 🔧 تشغيل المشروع

```bash
cd 05-dto-and-validations/DtoAndValidation
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`

## 🎓 أفضل الممارسات الموضحة

1. **التحقق من الإدخال** - لا تثق أبداً في إدخال العميل
2. **ثبات الاستجابة** - نفس التنسيق عبر جميع النقاط النهائية
3. **الأمان** - البيانات الداخلية لا تُعرض أبداً
4. **القابلية للصيانة** - سهولة تغيير النماذج الداخلية
5. **التوثيق** - عقود API واضحة في Swagger

## 🚨 سيناريوهات التحقق الشائعة

### **تحقق النص**
```csharp
[Required(ErrorMessage = "الاسم مطلوب")]
[StringLength(100, MinimumLength = 2)]
[RegularExpression(@"^[a-zA-Z0-9\s]+$", 
    ErrorMessage = "الاسم يحتوي على أحرف غير صحيحة")]
```

### **التحقق الرقمي**
```csharp
[Required]
[Range(0.01, 999999.99, ErrorMessage = "السعر يجب أن يكون بين $0.01 و $999,999.99")]
```

### **التحقق المخصص**
```csharp
[Required]
[RegularExpression(@"^(Electronics|Gaming|Accessories|Software)$", 
    ErrorMessage = "الفئة يجب أن تكون: Electronics, Gaming, Accessories, أو Software")]
```

## ➡️ الخطوات التالية

جاهز للتخلص من الربط اليدوي باستخدام AutoMapper؟
**[الفصل 06: تكامل AutoMapper](../06-automapper/)**

## 🤔 استكشاف الأخطاء وإصلاحها

**التحقق لا يعمل؟**
- تأكد من وجود خاصية `[ApiController]`
- تحقق من فحص ModelState.IsValid

**أخطاء التحقق مفقودة في الاستجابة؟**
- تحقق من إرجاع `BadRequest(ModelState)`
- تأكد من تعيين رسائل الخطأ بشكل صحيح

---

**💡 نصيحة احترافية**: تحقق دائماً عند حدود API - لا تثق أبداً في البيانات الواردة، حتى من العملاء الموثوقين!
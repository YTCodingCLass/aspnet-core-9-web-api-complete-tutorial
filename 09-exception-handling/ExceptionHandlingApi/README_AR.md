# معالجة الاستثناءات الشاملة مع RFC 7807 Problem Details

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Exception Handling](https://img.shields.io/badge/Exception_Handling-Middleware-FF6B35?style=flat-square)
![RFC 7807](https://img.shields.io/badge/RFC_7807-Problem_Details-2E8B57?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد درس معالجة الاستثناءات الشاملة](https://www.youtube.com/watch?v=Efmvo90NuiA&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=5)**

## 🎯 أهداف التعلم

بنهاية هذا الدرس، ستتقن:
- ✅ **معالجة الاستثناءات الشاملة** - معالجة الأخطاء المركزية باستخدام `IExceptionHandler`
- ✅ **RFC 7807 Problem Details** - استجابات خطأ موحدة ومعيارية
- ✅ **تسلسل استثناءات مخصص** - أنواع استثناءات منظمة مع أكواد حالة HTTP
- ✅ **سلسلة معالجات الاستثناءات** - معالجات محددة لأنواع مختلفة من الاستثناءات
- ✅ **التحقق في طبقة الخدمة** - نقل منطق التحقق من المتحكمات إلى الخدمات
- ✅ **معالجة أخطاء جاهزة للإنتاج** - تفاصيل خطأ تعتمد على البيئة

## 🚀 ما نبنيه

**نظام معالجة استثناءات جاهز للإنتاج** يوضح:
- **معالج استثناءات شامل** - يلتقط جميع الاستثناءات غير المعالجة
- **معالج استثناءات الأعمال** - يتعامل مع استثناءات المجال الخاصة
- **معالج استثناءات التحقق** - يتعامل مع أخطاء التحقق مع تفاصيل على مستوى الحقول
- **أنواع استثناءات مخصصة** - `NotFoundException`، `BadRequestException`، `ValidationException`، إلخ.
- **التحقق في طبقة الخدمة** - تحقق شامل في طبقة الخدمة
- **التوافق مع RFC 7807** - تنسيق تفاصيل المشكلة القياسي

## 📁 هيكل المشروع

```
ExceptionHandlingApi/
├── Controllers/
│   ├── ProductsController.cs        # متحكمات نحيفة بدون معالجة استثناءات
│   └── ServiceLifetimeController.cs # عرض توضيحي لعمر الخدمة
├── Exceptions/                       # ⭐ أنواع الاستثناءات المخصصة
│   ├── BaseException.cs             # الاستثناء الأساسي مع كود الحالة وكود الخطأ
│   ├── NotFoundException.cs         # 404 غير موجود
│   ├── BadRequestException.cs       # 400 طلب خاطئ
│   ├── ValidationException.cs       # 422 كيان غير قابل للمعالجة (مع قاموس أخطاء)
│   ├── UnauthorizedException.cs     # 401 غير مصرح
│   ├── ForbiddenException.cs        # 403 محظور
│   └── ConflictException.cs         # 409 تعارض
├── Handlers/                         # ⭐ معالجات الاستثناءات
│   ├── GlobalExceptionHandler.cs    # يلتقط جميع الاستثناءات غير المعالجة
│   ├── BusinessExceptionHandler.cs  # يتعامل مع أحفاد BaseException
│   └── ValidationExceptionHandler.cs # يتعامل مع أخطاء التحقق
├── Models/
│   ├── Product.cs                   # كيان المنتج
│   ├── Supplier.cs                  # كيان المورد
│   └── DTOs/
│       ├── CreateProductDto.cs      # طلب إنشاء المنتج
│       ├── UpdateProductDto.cs      # طلب تحديث المنتج
│       └── ProductResponseDto.cs    # استجابة المنتج مع حالة المخزون
├── Repositories/
│   ├── IProductRepository.cs        # واجهة المستودع
│   └── ProductRepository.cs         # تنفيذ المستودع
├── Services/
│   ├── IProductService.cs           # واجهة الخدمة
│   ├── ProductService.cs            # الخدمة مع منطق التحقق ⭐
│   ├── INotificationService.cs      # واجهة خدمة الإشعارات
│   └── NotificationService.cs       # تنفيذ الإشعارات
├── Data/
│   └── InMemoryDatabase.cs          # مخزن بيانات في الذاكرة
├── Mappings/
│   └── MappingProfile.cs            # تكوين AutoMapper
├── Program.cs                       # تسجيل معالجات الاستثناءات ⭐
└── ExceptionHandlingApi.http        # طلبات HTTP للاختبار
```

## 🏗️ بنية معالجة الاستثناءات

### **تدفق معالجة الاستثناءات**

```
┌─────────────────────────────────────────────────────┐
│            طلب HTTP (المتحكم)                       │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│         طبقة الخدمة (منطق الأعمال)                 │
│    • التحقق                                         │
│    • قواعد الأعمال                                 │
│    • معالجة البيانات                               │
└─────────────────┬───────────────────────────────────┘
                  │
                  │ يرمي استثناء
                  ▼
┌─────────────────────────────────────────────────────┐
│      UseExceptionHandler() Middleware               │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│    سلسلة معالجات الاستثناءات (الترتيب مهم!)       │
│    1. ValidationExceptionHandler ─┐                 │
│    2. BusinessExceptionHandler ───┼─► تطابق؟        │
│    3. GlobalExceptionHandler ─────┘                 │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│    استجابة JSON لتفاصيل المشكلة RFC 7807           │
│    {                                                 │
│      "type": "...",                                  │
│      "title": "...",                                 │
│      "status": 400,                                  │
│      "detail": "...",                                │
│      "errors": {...},                                │
│      "errorCode": "...",                             │
│      "timestamp": "...",                             │
│      "traceId": "..."                                │
│    }                                                 │
└─────────────────────────────────────────────────────┘
```

## 🎨 تسلسل الاستثناءات المخصص

### **الاستثناء الأساسي**
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

### **الاستثناءات المحددة**

| الاستثناء | حالة HTTP | كود الخطأ | حالة الاستخدام |
|-----------|------------|-----------|----------------|
| `NotFoundException` | 404 | `NOT_FOUND` | المورد غير موجود |
| `BadRequestException` | 400 | `BAD_REQUEST` | بيانات طلب غير صالحة أو انتهاك قاعدة عمل |
| `ValidationException` | 422 | `VALIDATION_ERROR` | فشل التحقق من الإدخال (مع أخطاء على مستوى الحقول) |
| `UnauthorizedException` | 401 | `UNAUTHORIZED` | المصادقة مطلوبة |
| `ForbiddenException` | 403 | `FORBIDDEN` | أذونات غير كافية |
| `ConflictException` | 409 | `CONFLICT` | تعارض المورد (مثل اسم مكرر) |

### **أمثلة على الاستثناءات**

```csharp
// تحقق حقل واحد
throw new ValidationException("Id", "يجب أن يكون معرف المنتج أكبر من صفر");

// تحقق حقول متعددة
var errors = new Dictionary<string, string[]>
{
    { "Name", ["اسم المنتج مطلوب"] },
    { "Price", ["يجب أن يكون السعر أكبر من صفر"] }
};
throw new ValidationException(errors);

// غير موجود
throw new NotFoundException("Product", id);

// انتهاك قاعدة عمل
throw new BadRequestException("يجب أن يكون السعر أعلى بنسبة 10% على الأقل من سعر التكلفة");

// تعارض
throw new ConflictException($"المنتج باسم '{name}' موجود بالفعل");
```

## ⚙️ التحقق في طبقة الخدمة

### **نقل التحقق من المتحكمات إلى الخدمات**

**❌ قبل (التحقق القائم على المتحكم):**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
{
    if (id <= 0)
    {
        throw new ValidationException("Id", "يجب أن يكون معرف المنتج أكبر من صفر");
    }

    var product = await productService.GetProductByIdAsync(id);
    if (product == null)
    {
        throw new NotFoundException("Product", id);
    }
    return Ok(product);
}
```

**✅ بعد (التحقق القائم على الخدمة):**
```csharp
// المتحكم - نحيف ونظيف
[HttpGet("{id}")]
public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
{
    var product = await productService.GetProductByIdAsync(id);
    return Ok(product);
}

// الخدمة - تتعامل مع كل التحقق
public async Task<ProductResponseDto> GetProductByIdAsync(int id)
{
    // التحقق
    if (id <= 0)
    {
        throw new ValidationException("Id", "يجب أن يكون معرف المنتج أكبر من صفر");
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

### **مثال تحقق شامل**

```csharp
private async Task ValidateProductCreationAsync(CreateProductDto createDto)
{
    var errors = new Dictionary<string, string[]>();

    // التحقق: اسم المنتج
    if (string.IsNullOrWhiteSpace(createDto.Name))
    {
        errors.Add("Name", ["اسم المنتج مطلوب"]);
    }
    else if (createDto.Name.Length < 3)
    {
        errors.Add("Name", ["يجب أن يكون اسم المنتج 3 أحرف على الأقل"]);
    }
    else if (createDto.Name.Length > 100)
    {
        errors.Add("Name", ["لا يمكن أن يتجاوز اسم المنتج 100 حرف"]);
    }

    // التحقق: السعر
    if (createDto.Price <= 0)
    {
        errors.Add("Price", ["يجب أن يكون السعر أكبر من صفر"]);
    }
    else if (createDto.Price > 1000000)
    {
        errors.Add("Price", ["لا يمكن أن يتجاوز السعر 1,000,000"]);
    }

    // التحقق: كمية المخزون
    if (createDto.StockQuantity < 0)
    {
        errors.Add("StockQuantity", ["لا يمكن أن تكون كمية المخزون سالبة"]);
    }

    // رمي ValidationException إذا كانت هناك أي أخطاء
    if (errors.Any())
    {
        throw new ValidationException(errors);
    }

    // قاعدة عمل: التحقق من وجود اسم المنتج بالفعل
    var existingProducts = await productRepository.GetAllAsync();
    if (existingProducts.Any(p => p.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
    {
        throw new ConflictException($"المنتج باسم '{createDto.Name}' موجود بالفعل");
    }

    // قاعدة عمل: التحقق من هامش الربح
    if (createDto.Price <= createDto.CostPrice * 1.1m)
    {
        throw new BadRequestException("يجب أن يكون السعر أعلى بنسبة 10% على الأقل من سعر التكلفة");
    }
}
```

## 🔌 تكوين Program.cs

```csharp
// تسجيل خدمة Problem Details
builder.Services.AddProblemDetails();

// تسجيل معالجات الاستثناءات بترتيب التحديد
// يجب تسجيل المعالجات الأكثر تحديداً أولاً
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// خدمات أخرى...
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// *** مهم: أضف معالج الاستثناءات في وقت مبكر من خط الأنابيب ***
app.UseExceptionHandler();

// وسيطة أخرى...
app.UseSwagger();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 📋 تنسيق استجابة RFC 7807 Problem Details

### **استجابة خطأ التحقق (422)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
  "title": "حدث خطأ تحقق واحد أو أكثر.",
  "status": 422,
  "detail": "حدث خطأ تحقق واحد أو أكثر.",
  "instance": "/api/products",
  "errors": {
    "Name": [
      "اسم المنتج مطلوب"
    ],
    "Price": [
      "يجب أن يكون السعر أكبر من صفر"
    ],
    "Category": [
      "الفئة مطلوبة"
    ]
  },
  "errorCode": "VALIDATION_ERROR",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **استجابة خطأ غير موجود (404)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
  "title": "غير موجود",
  "status": 404,
  "detail": "المنتج بمفتاح '999' غير موجود.",
  "instance": "/api/products/999",
  "errorCode": "NOT_FOUND",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **انتهاك قاعدة العمل (400)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
  "title": "طلب خاطئ",
  "status": 400,
  "detail": "يجب أن يكون السعر أعلى بنسبة 10% على الأقل من سعر التكلفة",
  "instance": "/api/products",
  "errorCode": "BAD_REQUEST",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **خطأ التعارض (409)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
  "title": "تعارض",
  "status": 409,
  "detail": "المنتج باسم 'Laptop Pro' موجود بالفعل",
  "instance": "/api/products",
  "errorCode": "CONFLICT",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

### **استثناء غير معالج - التطوير (500)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
  "title": "خطأ داخلي في الخادم",
  "status": 500,
  "detail": "لم يتم تعيين مرجع الكائن إلى مثيل كائن.",
  "instance": "/api/products",
  "errorCode": "INTERNAL_SERVER_ERROR",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00",
  "exceptionType": "NullReferenceException",
  "stackTrace": "at ExceptionHandlingApi.Services.ProductService.GetAllProductsAsync()..."
}
```

### **استثناء غير معالج - الإنتاج (500)**
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
  "title": "خطأ داخلي في الخادم",
  "status": 500,
  "detail": "حدث خطأ أثناء معالجة طلبك.",
  "instance": "/api/products",
  "errorCode": "INTERNAL_SERVER_ERROR",
  "timestamp": "2025-10-18T10:30:45.123Z",
  "traceId": "00-abc123-def456-00"
}
```

## 🧪 اختبار معالجة الاستثناءات

### **سيناريوهات الاختبار**

1. **خطأ التحقق** - اسم منتج فارغ
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "",
  "price": -10,
  "category": ""
}
```

2. **غير موجود** - معرف منتج غير صالح
```http
GET https://localhost:7xxx/api/products/99999
```

3. **انتهاك قاعدة العمل** - هامش ربح منخفض
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "منتج تجريبي",
  "price": 100,
  "costPrice": 95,
  "category": "اختبار"
}
```

4. **تعارض** - اسم منتج مكرر
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Laptop Pro",
  "price": 1200,
  "costPrice": 800,
  "category": "إلكترونيات"
}
```

5. **إنشاء جماعي مع التحقق**
```http
POST https://localhost:7xxx/api/products/bulk-create
Content-Type: application/json

[
  {
    "name": "منتج 1",
    "price": 100
  },
  {
    "name": "منتج 1",
    "price": 100
  }
]
```

## 🎓 الفوائد الرئيسية

### **1. معالجة أخطاء مركزية**
- ✅ جميع الاستثناءات معالجة في مكان واحد
- ✅ لا توجد كتل try-catch في المتحكمات
- ✅ تنسيق استجابة خطأ متسق
- ✅ سهل الصيانة والتوسيع

### **2. التوافق مع RFC 7807**
- ✅ تنسيق تفاصيل المشكلة القياسي
- ✅ استجابات خطأ قابلة للقراءة آلياً
- ✅ تكامل أفضل للعميل
- ✅ أفضل ممارسات الصناعة

### **3. متحكمات نظيفة**
- ✅ متحكمات نحيفة بدون منطق معالجة الاستثناءات
- ✅ التركيز على مخاوف HTTP فقط
- ✅ قابلية قراءة وصيانة أفضل

### **4. التحقق في طبقة الخدمة**
- ✅ منطق تحقق قابل لإعادة الاستخدام
- ✅ يعمل عبر نقاط دخول مختلفة (REST، gRPC، إلخ.)
- ✅ فصل أفضل للمخاوف
- ✅ أسهل للاختبار

### **5. الوعي بالبيئة**
- ✅ معلومات خطأ مفصلة في التطوير
- ✅ استجابات معقمة في الإنتاج
- ✅ أفضل ممارسات الأمان

### **6. تسلسل استثناءات منظم**
- ✅ أنواع استثناءات واضحة لسيناريوهات مختلفة
- ✅ أكواد حالة HTTP مدمجة
- ✅ أكواد خطأ مخصصة لمعالجة العميل
- ✅ معالجة استثناءات آمنة من حيث النوع

## 🔧 تشغيل المشروع

```bash
cd ExceptionHandlingApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`

## 🎯 النقاط الرئيسية

1. **معالجة الاستثناءات الشاملة**: استخدم `IExceptionHandler` لمعالجة الأخطاء المركزية
2. **RFC 7807**: اتبع تنسيق تفاصيل المشكلة القياسي لأخطاء API
3. **الاستثناءات المخصصة**: إنشاء تسلسل استثناءات منظم مع أكواد الحالة
4. **سلسلة المعالج**: تسجيل المعالجات من الأكثر تحديداً إلى الأكثر عمومية
5. **التحقق من الخدمة**: نقل منطق التحقق من المتحكمات إلى الخدمات
6. **الوعي بالبيئة**: إظهار أخطاء مفصلة في التطوير، معقمة في الإنتاج
7. **متحكمات نظيفة**: حافظ على المتحكمات نحيفة ومركزة على مخاوف HTTP

## ➡️ الخطوة التالية

**قم بتوسيع معالجة الاستثناءات هذه مع:**
- **تكامل التسجيل** - تسجيل الاستثناءات في ملف أو قاعدة بيانات أو سحابة
- **تتبع الأخطاء** - التكامل مع Sentry، Application Insights، إلخ.
- **صفحات خطأ مخصصة** - صفحات خطأ ودية لتطبيقات الويب
- **استثناءات تحديد المعدل** - معالجة 429 Too Many Requests
- **قاطع الدائرة** - معالجة عدم توفر الخدمة بأمان
- **سياسات إعادة المحاولة** - إعادة المحاولة التلقائية مع Polly

---

**💡 نصيحة احترافية**: تحقق دائماً في طبقة الخدمة، وليس في المتحكمات. هذا يجعل التحقق قابلاً لإعادة الاستخدام عبر نقاط دخول مختلفة وأسهل للاختبار!

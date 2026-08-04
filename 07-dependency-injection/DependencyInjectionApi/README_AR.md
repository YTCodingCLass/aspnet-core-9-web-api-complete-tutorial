# الفصل 07: حقن التبعية - فهم دورات حياة الخدمات

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Dependency Injection](https://img.shields.io/badge/Dependency_Injection-DI-FF6B35?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد الفصل 07: حقن التبعية](https://www.youtube.com/watch?v=8Q0cU9Gq8Co&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=7)**

## 🎯 أهداف التعلم

في نهاية هذا الفصل، ستتقن:
- ✅ فهم مبادئ حقن التبعية في ASP.NET Core
- ✅ إنشاء وتسجيل الخدمات في حاوي DI
- ✅ دورات حياة الخدمات: **Transient** و **Scoped** و **Singleton**
- ✅ الفروق العملية بين دورات حياة الخدمات
- ✅ متى تستخدم كل دورة حياة في التطبيقات الحقيقية

## 🚀 ما سنبنيه

**API توضيحي لدورات حياة الخدمات** يوضح:
- **خدمات Singleton** - نفس النسخة عبر التطبيق بالكامل
- **خدمات Scoped** - نفس النسخة ضمن طلب HTTP واحد
- **خدمات Transient** - نسخة جديدة في كل مرة يتم طلبها
- **عرض بصري** لكيفية تصرف كل دورة حياة
- **تسجيل في وحدة التحكم** لتتبع إنشاء النسخ

## 📁 هيكل المشروع

```
DependencyInjectionApi/
├── Controllers/
│   ├── ProductsController.cs        # منتجات API مع AutoMapper
│   └── ServiceLifetimeController.cs # عرض دورات حياة الخدمات
├── Models/
│   ├── Product.cs              # نموذج كيان المنتج
│   └── DTOs/
│       ├── CreateProductDto.cs     # DTOs الطلبات
│       ├── UpdateProductDto.cs     
│       ├── PatchProductDto.cs      
│       └── ProductResponseDto.cs   # DTO الاستجابة مع حالة المخزون
├── Services.cs                 # واجهات وتطبيقات الخدمات
├── Mappings/
│   └── MappingProfile.cs       # تكوين AutoMapper
├── Program.cs                  # تسجيل حاوي DI
└── DependencyInjectionApi.http # طلبات HTTP للاختبار
```

## 🔧 فهم دورات حياة الخدمات

### **1. واجهات وتطبيقات الخدمات**
```csharp
// Services.cs
namespace DependencyInjectionApi;

// تعاريف الواجهات
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

// تطبيقات الخدمات مع التسجيل
public class SingletonService : ISingletonService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public SingletonService()
    {
        Console.WriteLine($"🔴 تم إنشاء Singleton: {InstanceId}");
    }
}

public class ScopedService : IScopedService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public ScopedService()
    {
        Console.WriteLine($"🟡 تم إنشاء Scoped: {InstanceId}");
    }
}

public class TransientService : ITransientService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public TransientService()
    {
        Console.WriteLine($"🟢 تم إنشاء Transient: {InstanceId}");
    }
}
```

### **2. تسجيل الخدمات في حاوي DI**
```csharp
// Program.cs
builder.Services.AddSingleton<ISingletonService, SingletonService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddTransient<ITransientService, TransientService>();
```

## 💻 كونترولر دورات حياة الخدمات

### **عرض دورات حياة الخدمات**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ServiceLifetimeController : ControllerBase
{
    // حقن نسختين من كل نوع خدمة
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
        
        Console.WriteLine("🏗️ تم إنشاء الكونترولر مع جميع الخدمات");
    }
    
    [HttpGet("demo")]
    public ActionResult<object> GetServiceLifetimeDemo()
    {
        return Ok(new
        {
            Explanation = new
            {
                Singleton = "نفس النسخة عبر دورة حياة التطبيق بالكامل",
                Scoped = "نفس النسخة ضمن طلب HTTP واحد", 
                Transient = "نسخة جديدة في كل مرة يتم طلب الخدمة"
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

## 🎛️ سلوك دورات حياة الخدمات

### **🔴 دورة حياة Singleton**
- **يتم إنشاؤه مرة واحدة** عند أول طلب
- **نفس النسخة** مشتركة عبر التطبيق بالكامل
- **يعيش حتى** إغلاق التطبيق
- **استخدمه لـ**: الخدمات عديمة الحالة، التخزين المؤقت، التكوين

```csharp
builder.Services.AddSingleton<ISingletonService, SingletonService>();
```

### **🟡 دورة حياة Scoped**
- **يتم إنشاؤه مرة واحدة** لكل طلب HTTP
- **نفس النسخة** ضمن نفس الطلب
- **يتم التخلص منه** عند انتهاء الطلب
- **استخدمه لـ**: سياقات قاعدة البيانات، الخدمات الخاصة بالطلب

```csharp
builder.Services.AddScoped<IScopedService, ScopedService>();
```

### **🟢 دورة حياة Transient**
- **يتم إنشاؤه في كل مرة** يتم طلب الخدمة
- **نسخة جديدة** لكل حقن
- **يتم التخلص منه** عند انتهاء النطاق
- **استخدمه لـ**: الخدمات خفيفة الوزن وعديمة الحالة

```csharp
builder.Services.AddTransient<ITransientService, TransientService>();
```

## 🧪 اختبار دورات حياة الخدمات

### **اختبار عرض دورة حياة الخدمة**
```http
GET https://localhost:7xxx/api/servicelifetime/demo
```

### **الاستجابة المتوقعة**
```json
{
  "explanation": {
    "singleton": "نفس النسخة عبر دورة حياة التطبيق بالكامل",
    "scoped": "نفس النسخة ضمن طلب HTTP واحد",
    "transient": "نسخة جديدة في كل مرة يتم طلب الخدمة"
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

### **مخرجات وحدة التحكم**
```
🔴 تم إنشاء Singleton: 550e8400-e29b-41d4-a716-446655440000
🟡 تم إنشاء Scoped: 6ba7b810-9dad-11d1-80b4-00c04fd430c8
🟢 تم إنشاء Transient: 6ba7b811-9dad-11d1-80b4-00c04fd430c8
🟢 تم إنشاء Transient: 6ba7b812-9dad-11d1-80b4-00c04fd430c8
🏗️ تم إنشاء الكونترولر مع جميع الخدمات
```

## 🎓 الملاحظات الرئيسية

### **1. سلوك Singleton**
- ✅ **Instance1_Id == Instance2_Id** (نفس الـ GUID)
- ✅ **يتم إنشاؤه مرة واحدة فقط** عند بدء التطبيق
- ✅ **مشترك عبر جميع الطلبات** والكونترولرز

### **2. سلوك Scoped**
- ✅ **Instance1_Id == Instance2_Id** ضمن نفس الطلب
- ✅ **نسخة جديدة لكل طلب HTTP**
- ✅ **يتم التخلص منه عند اكتمال الطلب**

### **3. سلوك Transient**
- ❌ **Instance1_Id != Instance2_Id** (GUIDs مختلفة)
- ✅ **نسخة جديدة في كل حقن**
- ✅ **نسختان مختلفتان** في نفس الكونترولر

### **4. تأثيرات الأداء**
- **Singleton**: الأسرع، بدون overhead للتخصيص
- **Scoped**: متوسط، تخصيص واحد لكل طلب
- **Transient**: الأبطأ، تخصيص في كل حقن

## 🔧 تشغيل المشروع

```bash
cd 07-dependency-injection/DependencyInjectionApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**عرض الخدمة**: `https://localhost:7xxx/api/servicelifetime/demo`

## 🏗️ إرشادات الاستخدام في العالم الحقيقي

### **متى تستخدم Singleton**
```csharp
// ✅ مرشحون جيدون
builder.Services.AddSingleton<IConfiguration>();
builder.Services.AddSingleton<ILogger<T>>();
builder.Services.AddSingleton<ICacheService>();
builder.Services.AddSingleton<IEmailService>();
```

### **متى تستخدم Scoped**
```csharp
// ✅ مرشحون جيدون
builder.Services.AddScoped<DbContext>();
builder.Services.AddScoped<IUserService>();
builder.Services.AddScoped<IOrderService>();
builder.Services.AddScoped<IUnitOfWork>();
```

### **متى تستخدم Transient**
```csharp
// ✅ مرشحون جيدون
builder.Services.AddTransient<IValidator<T>>();
builder.Services.AddTransient<IMapper>();
builder.Services.AddTransient<IHttpClientFactory>();
builder.Services.AddTransient<IDateTimeProvider>();
```

### **⚠️ الأخطاء الشائعة**
- **لا تحقن** خدمات Scoped في Singletons
- **لا تخزن حالة** في خدمات Transient
- **لا تستخدم Transient** للكائنات مكلفة الإنشاء

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

1. **Singleton**: نسخة واحدة لدورة حياة التطبيق بالكامل
2. **Scoped**: نسخة واحدة لكل طلب HTTP
3. **Transient**: نسخة جديدة في كل مرة يتم حقنها
4. **اختر دورة الحياة بناءً على إدارة الحالة واحتياجات الأداء**
5. **كن حذراً مع التبعيات الأسيرة**
6. **استخدم تسجيل وحدة التحكم لتصور إنشاء النسخ**

## ➡️ ماذا بعد؟

**مسارات التعلم التالية:**
- **نمط Repository** مع حقن التبعية
- **Entity Framework Core** مع تسجيل الخدمة المناسب
- **اختبار الوحدة** مع التبعيات المموهة
- **خدمات المصادقة والتفويض**
- **الخدمات الخلفية** والخدمات المستضافة

## 🤔 استكشاف الأخطاء وإصلاحها

**استثناء خدمة غير موجودة؟**
- تحقق من تسجيل الخدمة في Program.cs
- تحقق من أن الواجهة والتطبيق مسجلان بشكل صحيح

**تم اكتشاف تبعية أسيرة؟**
- لا تحقن خدمات قصيرة المدى في خدمات طويلة المدى
- استخدم نمط factory أو service locator للسيناريوهات المعقدة

**تسريبات ذاكرة مع الخدمات؟**
- طبق IDisposable للخدمات التي تحتفظ بموارد
- كن حذراً مع اشتراكات الأحداث في خدمات singleton

---

**💡 نصيحة احترافية**: استخدم نقطة النهاية `/demo` لفهم سلوك الخدمة قبل بناء تطبيقات معقدة!
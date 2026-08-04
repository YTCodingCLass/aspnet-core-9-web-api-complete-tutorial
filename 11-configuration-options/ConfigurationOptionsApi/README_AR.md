# التكوين ونمط Options في ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Configuration](https://img.shields.io/badge/Configuration-Options_Pattern-FF6B35?style=flat-square)
![Settings](https://img.shields.io/badge/App-Settings-2E8B57?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد درس التكوين ونمط Options](https://www.youtube.com/watch?v=S3ygXq94dLo&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=3)**

## 🎯 أهداف التعلم

بنهاية هذا الدرس، ستتقن:
- ✅ **نظام التكوين** - فهم التسلسل الهرمي للتكوين في ASP.NET Core
- ✅ **نمط Options** - التكوين قوي النوع باستخدام `IOptions<T>`
- ✅ **appsettings.json** - إدارة ملفات التكوين لبيئات مختلفة
- ✅ **ربط التكوين** - ربط تكوين JSON بكلاسات C#
- ✅ **Middleware قابل للتكوين** - جعل سلوك middleware قابل للتكوين
- ✅ **إعدادات خاصة بالبيئة** - تكوين التطوير مقابل الإنتاج
- ✅ **أفضل ممارسات التكوين** - إدارة تكوين آمنة وقابلة للصيانة

## 🚀 ما نبنيه

**نظام تكوين جاهز للإنتاج** يتميز بـ:

1. **RequestResponseLoggingOptions** - كلاس تكوين قوي النوع
2. **Middleware قابل للتكوين** - Middleware يقرأ الإعدادات من appsettings.json
3. **تكوين خاص بالبيئة** - إعدادات مختلفة للتطوير والإنتاج
4. **تنفيذ نمط Options** - حقن التبعية لإعدادات التكوين

## 📁 هيكل المشروع

```
ConfigurationOptionsApi/
├── Controllers/
│   └── ProductsController.cs        # نقاط نهاية API
├── Configuration/                    # ⭐ كلاسات التكوين
│   └── RequestResponseLoggingOptions.cs # كلاس options قوي النوع ⭐
├── Middleware/                       # مكونات middleware مخصصة
│   ├── RequestLoggingMiddleware.cs  # تسجيل طلبات واستجابات HTTP
│   ├── ResponseTimingMiddleware.cs  # قياس وتسجيل وقت الاستجابة
│   └── RequestResponseLoggingMiddleware.cs # middleware تسجيل قابل للتكوين ⭐
├── Exceptions/                       # أنواع الاستثناءات المخصصة
│   ├── BaseException.cs             # الاستثناء الأساسي مع كود الحالة
│   ├── NotFoundException.cs         # 404 غير موجود
│   ├── BadRequestException.cs       # 400 طلب خاطئ
│   ├── ValidationException.cs       # 422 كيان غير قابل للمعالجة
│   ├── UnauthorizedException.cs     # 401 غير مصرح
│   ├── ForbiddenException.cs        # 403 محظور
│   └── ConflictException.cs         # 409 تعارض
├── Handlers/                         # معالجات الاستثناءات
│   ├── GlobalExceptionHandler.cs    # يلتقط جميع الاستثناءات غير المعالجة
│   ├── BusinessExceptionHandler.cs  # يتعامل مع استثناءات الأعمال
│   └── ValidationExceptionHandler.cs # يتعامل مع أخطاء التحقق
├── Models/
│   ├── Product.cs                   # كيان المنتج
│   ├── Supplier.cs                  # كيان المورد
│   └── DTOs/
│       └── ProductDtos.cs           # DTOs المنتج
├── Repositories/
│   ├── IProductRepository.cs        # واجهة المستودع
│   └── ProductRepository.cs         # تنفيذ المستودع
├── Services/
│   ├── IProductService.cs           # واجهة الخدمة
│   ├── ProductService.cs            # الخدمة مع منطق التحقق
│   ├── INotificationService.cs      # واجهة خدمة الإشعارات
│   └── NotificationService.cs       # تنفيذ الإشعارات
├── Data/
│   └── InMemoryDatabase.cs          # مخزن بيانات في الذاكرة
├── Mappings/
│   └── MappingProfile.cs            # تكوين AutoMapper
├── Program.cs                       # إعداد التكوين و DI ⭐
├── appsettings.json                 # ملف التكوين الأساسي ⭐
├── appsettings.Development.json     # إعدادات خاصة بالتطوير ⭐
└── ConfigurationOptionsApi.http     # طلبات HTTP للاختبار
```

## 🏗️ بنية نظام التكوين

### **تدفق التسلسل الهرمي للتكوين**

```
┌─────────────────────────────────────────────────────┐
│          مصادر التكوين (الأولوية)                  │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│  1. appsettings.json (التكوين الأساسي)             │
│     • إعدادات مشتركة لجميع البيئات                 │
│     • القيم الافتراضية                             │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│  2. appsettings.{Environment}.json                  │
│     • تجاوزات خاصة بالبيئة                         │
│     • Development, Staging, Production              │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│  3. متغيرات البيئة (أعلى أولوية)                   │
│     • تكوين الحاويات/السحابة                       │
│     • الأسرار والبيانات الحساسة                    │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│           Configuration Builder                     │
│     • يدمج جميع المصادر                            │
│     • المصادر اللاحقة تتجاوز المصادر السابقة       │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│         نمط Options (IOptions<T>)                   │
│     • كلاسات تكوين قوية النوع                      │
│     • حقن التبعية                                  │
│     • وصول آمن من حيث النوع للإعدادات              │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│        مكونات التطبيق                              │
│     • Middleware                                    │
│     • الخدمات                                      │
│     • الكونترولرز                                  │
└─────────────────────────────────────────────────────┘
```

## 💻 تنفيذ التكوين

### **الخطوة 1: إنشاء كلاس Options قوي النوع**

```csharp
// Configuration/RequestResponseLoggingOptions.cs
namespace ConfigurationOptionsApi.Configuration;

/// <summary>
/// خيارات التكوين لـ RequestResponseLoggingMiddleware.
/// يوضح نمط Options لجعل middleware قابل للتكوين.
/// </summary>
public class RequestResponseLoggingOptions
{
    /// <summary>
    /// اسم قسم التكوين في appsettings.json
    /// </summary>
    public const string SectionName = "RequestResponseLogging";

    /// <summary>
    /// تفعيل أو تعطيل تسجيل تفصيلي للطلبات/الاستجابات
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// تضمين headers الطلب في السجلات
    /// </summary>
    public bool IncludeRequestHeaders { get; set; } = false;

    /// <summary>
    /// تضمين headers الاستجابة في السجلات
    /// </summary>
    public bool IncludeResponseHeaders { get; set; } = false;

    /// <summary>
    /// تضمين محتوى الطلب في السجلات
    /// </summary>
    public bool IncludeRequestBody { get; set; } = true;

    /// <summary>
    /// تضمين محتوى الاستجابة في السجلات
    /// </summary>
    public bool IncludeResponseBody { get; set; } = true;

    /// <summary>
    /// الحد الأقصى لحجم المحتوى للتسجيل (بالبايت). المحتويات الأكبر ستُقتطع.
    /// </summary>
    public int MaxBodySizeToLog { get; set; } = 4096;
}
```

**المفاهيم الرئيسية:**
- **ثابت SectionName** - يحدد اسم قسم التكوين
- **القيم الافتراضية** - يوفر قيم افتراضية معقولة لجميع الخصائص
- **توثيق XML** - أوصاف واضحة لكل إعداد
- **أمان النوع** - خصائص قوية النوع بدلاً من السلاسل النصية السحرية

---

### **الخطوة 2: تعريف التكوين في appsettings.json**

```json
// appsettings.json - التكوين الأساسي
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
// appsettings.Development.json - تجاوزات التطوير
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
// appsettings.Production.json - تجاوزات الإنتاج
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

**المفاهيم الرئيسية:**
- **تكوين هرمي** - إعدادات أساسية مع تجاوزات البيئة
- **بنية JSON** - تطابق أسماء خصائص كلاس C#
- **خاص بالبيئة** - إعدادات مختلفة للتطوير مقابل الإنتاج
- **الأمان** - تعطيل التسجيل المطول في الإنتاج

---

### **الخطوة 3: تسجيل Options في Program.cs**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// ========================================
// تكوين نمط Options
// ========================================
// ربط أقسام التكوين بكلاسات options قوية النوع
// يوضح هذا نمط Options لـ middleware قابل للتكوين
builder.Services.Configure<RequestResponseLoggingOptions>(
    builder.Configuration.GetSection(RequestResponseLoggingOptions.SectionName));

// تسجيلات خدمات أخرى...
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// تسجيل خدمات middleware
builder.Services.AddScoped<RequestResponseLoggingMiddleware>();
builder.Services.AddScoped<ResponseTimingMiddleware>();
builder.Services.AddScoped<RequestLoggingMiddleware>();

var app = builder.Build();

// تكوين خط أنابيب middleware
app.UseExceptionHandler();
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // يستخدم IOptions<T>
app.UseMiddleware<ResponseTimingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();
```

**المفاهيم الرئيسية:**
- **Configure<T>()** - يسجل options مع حاوي DI
- **GetSection()** - يسترجع قسم التكوين بالاسم
- **قوة النوع** - يُربط التكوين بـ `RequestResponseLoggingOptions`
- **حقن التبعية** - يُحقن Options في المكونات

---

### **الخطوة 4: استهلاك Options في Middleware**

```csharp
// Middleware/RequestResponseLoggingMiddleware.cs
using Microsoft.Extensions.Options;

public class RequestResponseLoggingMiddleware(
    ILogger<RequestResponseLoggingMiddleware> logger,
    IOptions<RequestResponseLoggingOptions> options)  // ⭐ حقن IOptions<T>
    : IMiddleware
{
    private readonly RequestResponseLoggingOptions _options = options.Value; // ⭐ الحصول على القيمة

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // التحقق إذا كان التسجيل مفعلاً عبر التكوين
        if (!_options.IsEnabled)
        {
            await next(context);
            return;
        }

        // تسجيل الطلب
        await LogRequest(context);

        // نسخ stream الاستجابة الأصلي
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        // تنفيذ middleware التالي
        await next(context);

        // تسجيل الاستجابة
        await LogResponse(context);

        // نسخ المحتويات إلى stream الأصلي
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task LogRequest(HttpContext context)
    {
        var logBuilder = new StringBuilder();
        logBuilder.AppendLine("معلومات طلب HTTP:");
        logBuilder.AppendLine($"الطريقة: {context.Request.Method}");
        logBuilder.AppendLine($"المسار: {context.Request.Path}");

        // تضمين headers إذا تم تكوينها
        if (_options.IncludeRequestHeaders)
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in context.Request.Headers)
            {
                logBuilder.AppendLine($"  {header.Key}: {header.Value}");
            }
        }

        // تضمين المحتوى إذا تم تكوينه
        if (_options.IncludeRequestBody)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            // القطع إذا تجاوز المحتوى الحد الأقصى
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [مُقتطع]";
            }

            logBuilder.AppendLine($"المحتوى: {body}");
        }

        logger.LogInformation(logBuilder.ToString());
    }

    private async Task LogResponse(HttpContext context)
    {
        var logBuilder = new StringBuilder();
        logBuilder.AppendLine("معلومات استجابة HTTP:");
        logBuilder.AppendLine($"كود الحالة: {context.Response.StatusCode}");

        // تضمين headers إذا تم تكوينها
        if (_options.IncludeResponseHeaders)
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in context.Response.Headers)
            {
                logBuilder.AppendLine($"  {header.Key}: {header.Value}");
            }
        }

        // تضمين المحتوى إذا تم تكوينه
        if (_options.IncludeResponseBody)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // القطع إذا تجاوز المحتوى الحد الأقصى
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [مُقتطع]";
            }

            logBuilder.AppendLine($"المحتوى: {body}");
        }

        logger.LogInformation(logBuilder.ToString());
    }
}
```

**المفاهيم الرئيسية:**
- **حقن IOptions<T>** - يُحقن Options عبر الـ constructor
- **options.Value** - الوصول إلى الإعدادات المكونة
- **سلوك مدفوع بالتكوين** - سلوك Middleware يتغير بناءً على الإعدادات
- **تكوين وقت التشغيل** - لا حاجة لتغييرات كود لتعديل السلوك
- **حد حجم المحتوى** - يمنع تسجيل أحمال ضخمة

---

## 🎨 فوائد نمط Options

### **قبل: تكوين ثابت في الكود** ❌

```csharp
public class RequestResponseLoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // يسجل كل شيء دائماً - بدون مرونة
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        logger.LogInformation($"محتوى الطلب: {body}");

        await next(context);
    }
}
```

**المشاكل:**
- ❌ لا يوجد طريقة لتعطيل التسجيل
- ❌ يسجل المحتويات الكاملة دائماً (يمكن أن تكون ضخمة!)
- ❌ لا يمكن تبديل headers تشغيل/إيقاف
- ❌ يتطلب تغييرات كود لتعديل السلوك

---

### **بعد: نمط Options** ✅

```csharp
public class RequestResponseLoggingMiddleware(
    ILogger<RequestResponseLoggingMiddleware> logger,
    IOptions<RequestResponseLoggingOptions> options) : IMiddleware
{
    private readonly RequestResponseLoggingOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // التحقق إذا كان مفعلاً عبر التكوين
        if (!_options.IsEnabled)
        {
            await next(context);
            return;
        }

        // التسجيل فقط إذا تم تكوينه
        if (_options.IncludeRequestBody)
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

            // القطع إذا لزم الأمر
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [مُقتطع]";
            }

            logger.LogInformation($"محتوى الطلب: {body}");
        }

        await next(context);
    }
}
```

**الفوائد:**
- ✅ تفعيل/تعطيل عبر appsettings.json
- ✅ حدود حجم المحتوى لمنع السجلات الضخمة
- ✅ تبديل headers/المحتوى بشكل مستقل
- ✅ تكوين خاص بالبيئة
- ✅ لا حاجة لتغييرات كود

---

## 🔧 أنماط التكوين

### **النمط 1: ربط تكوين بسيط**

```csharp
// تسجيل options
builder.Services.Configure<MyOptions>(
    builder.Configuration.GetSection("MySection"));

// الاستهلاك في خدمة
public class MyService(IOptions<MyOptions> options)
{
    private readonly MyOptions _options = options.Value;
}
```

### **النمط 2: تكوين مع التحقق**

```csharp
// كلاس options مع التحقق
public class ApiKeyOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

// التسجيل مع التحقق
builder.Services.AddOptions<ApiKeyOptions>()
    .Bind(builder.Configuration.GetSection("ApiKey"))
    .Validate(options => !string.IsNullOrEmpty(options.ApiKey),
              "مفتاح API مطلوب");
```

### **النمط 3: مصادر تكوين متعددة**

```csharp
// بناء تكوين من مصادر متعددة
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()  // أسرار التطوير
    .Build();
```

### **النمط 4: IOptionsSnapshot للتكوين القابل لإعادة التحميل**

```csharp
// استخدام IOptionsSnapshot بدلاً من IOptions للتكوين القابل لإعادة التحميل
public class MyService(IOptionsSnapshot<MyOptions> options)
{
    // يُعاد تقييم Options في كل طلب إذا تغير ملف التكوين
    private MyOptions GetCurrentOptions() => options.Value;
}
```

## 🧪 اختبار التكوين

### **اختبار قيم تكوين مختلفة**

```http
### اختبار مع IsEnabled = true (الافتراضي)
GET https://localhost:7xxx/api/products
```

**المتوقع:** تسجيل كامل للطلب/الاستجابة في وحدة التحكم

---

```http
### تغيير appsettings.json: IsEnabled = false
GET https://localhost:7xxx/api/products
```

**المتوقع:** لا يوجد تسجيل تفصيلي

---

```http
### تغيير IncludeRequestHeaders = true
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "منتج تجريبي",
  "price": 99.99
}
```

**المتوقع:** تضمين Headers في السجلات

---

## 🎓 الفوائد الرئيسية

### **1. أمان النوع**
- ✅ كلاسات تكوين قوية النوع
- ✅ فحص وقت الترجمة
- ✅ دعم IntelliSense
- ✅ أمان إعادة البناء

### **2. قابلية الصيانة**
- ✅ تكوين مركزي
- ✅ بنية واضحة مع توثيق XML
- ✅ قيم افتراضية في الكود
- ✅ سهل الفهم والتعديل

### **3. إدارة البيئة**
- ✅ إعدادات أساسية مع تجاوزات
- ✅ تكوينات التطوير مقابل الإنتاج
- ✅ جاهز للحاويات/السحابة
- ✅ دعم إدارة الأسرار

### **4. المرونة**
- ✅ تغيير السلوك بدون تغييرات كود
- ✅ تبديل الميزات تشغيل/إيقاف
- ✅ ضبط الحدود والعتبات
- ✅ تحديثات تكوين وقت التشغيل (مع IOptionsSnapshot)

### **5. قابلية الاختبار**
- ✅ سهل محاكاة IOptions<T>
- ✅ حقن تكوينات اختبار
- ✅ اختبار الوحدة مع إعدادات مختلفة
- ✅ تكوينات بيئة اختبار التكامل

---

## 🔧 تشغيل المشروع

```bash
cd 11-configuration-options/ConfigurationOptionsApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`

### **اختبار بيئات مختلفة**

```bash
# التشغيل مع بيئة التطوير (يستخدم appsettings.Development.json)
dotnet run --environment Development

# التشغيل مع بيئة الإنتاج (يستخدم appsettings.Production.json)
dotnet run --environment Production

# التشغيل مع بيئة مخصصة
dotnet run --environment Staging
```

---

## 🎯 النقاط الرئيسية

1. **نمط Options**: استخدم `IOptions<T>` للتكوين قوي النوع
2. **التسلسل الهرمي للتكوين**: appsettings.json → appsettings.{Environment}.json → متغيرات البيئة
3. **أمان النوع**: كلاسات التكوين توفر فحص وقت الترجمة
4. **خاص بالبيئة**: إعدادات مختلفة للتطوير والمرحلة والإنتاج
5. **حقن التبعية**: يُحقن Options في الخدمات و middleware
6. **تكوين قابل لإعادة التحميل**: استخدم `IOptionsSnapshot<T>` للتكوينات التي تتغير وقت التشغيل
7. **التحقق**: أضف التحقق من التكوين للإعدادات الحرجة
8. **الأمان**: احفظ الأسرار في متغيرات البيئة أو Key Vault، وليس في appsettings.json

---

## 🔒 أفضل ممارسات أمان التكوين

### **❌ لا تحفظ الأسرار أبداً في appsettings.json**

```json
// لا تفعل هذا!
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod;Database=MyDb;User=admin;Password=Pa$$w0rd123"
  },
  "ApiKeys": {
    "PaymentGateway": "sk_live_abc123xyz789"
  }
}
```

### **✅ استخدم متغيرات البيئة أو مديري الأسرار**

```csharp
// التطوير: User Secrets
dotnet user-secrets init
dotnet user-secrets set "ApiKeys:PaymentGateway" "sk_test_abc123"

// الإنتاج: متغيرات البيئة
// عيّن في Azure App Service، Kubernetes، Docker، إلخ
export ApiKeys__PaymentGateway="sk_live_xyz789"
```

```csharp
// الوصول إلى الأسرار بنفس طريقة التكوين العادي
builder.Services.Configure<ApiKeyOptions>(
    builder.Configuration.GetSection("ApiKeys"));
```

---

## ➡️ ماذا بعد؟

**توسيع نظام التكوين هذا بـ:**
- **Azure Key Vault** - حفظ الأسرار في Azure Key Vault
- **التحقق من التكوين** - التحقق من الإعدادات عند بدء التشغيل
- **IOptionsSnapshot** - تكوين قابل لإعادة التحميل بدون إعادة تشغيل
- **IOptionsMonitor** - تتبع تغييرات التكوين مع callbacks
- **مزودي تكوين مخصصين** - تحميل التكوين من قاعدة بيانات، APIs، إلخ
- **أعلام الميزات** - تبديل الميزات ديناميكياً
- **تشفير التكوين** - تشفير أقسام حساسة

---

## 💡 نصائح احترافية

1. **استخدم const لأسماء الأقسام** - يمنع الأخطاء المطبعية ويمكّن إعادة البناء
   ```csharp
   public const string SectionName = "RequestResponseLogging";
   ```

2. **وفر قيم افتراضية** - ابدأ الخصائص بقيم افتراضية معقولة
   ```csharp
   public bool IsEnabled { get; set; } = false;
   ```

3. **وثّق التكوين** - استخدم تعليقات XML لجميع options
   ```csharp
   /// <summary>
   /// الحد الأقصى لحجم المحتوى للتسجيل (بالبايت)
   /// </summary>
   public int MaxBodySizeToLog { get; set; } = 4096;
   ```

4. **تحقق من التكوين** - أضف التحقق عند بدء التشغيل للإعدادات الحرجة
   ```csharp
   builder.Services.AddOptions<MyOptions>()
       .Validate(o => o.MaxSize > 0, "MaxSize يجب أن يكون موجباً");
   ```

5. **تكوينات خاصة بالبيئة** - استخدم إعدادات مختلفة لكل بيئة
   - التطوير: تسجيل مطول، وضع التصحيح
   - الإنتاج: تسجيل أدنى، إعدادات محسّنة

---

**💡 نصيحة احترافية**: نمط Options هو الطريقة الموصى بها للوصول إلى التكوين في ASP.NET Core. يوفر أمان النوع وحقن التبعية والمرونة دون التضحية بالأداء!

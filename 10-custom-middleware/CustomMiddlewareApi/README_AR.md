# Custom Middleware في ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Middleware](https://img.shields.io/badge/Custom-Middleware-FF6B35?style=flat-square)
![Pipeline](https://img.shields.io/badge/Request-Pipeline-2E8B57?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد درس Custom Middleware](https://www.youtube.com/watch?v=n1A_IjEf_hs&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=4)**

## 🎯 أهداف التعلم

بنهاية هذا الدرس، ستتقن:
- ✅ **Custom Middleware** - إنشاء middleware مخصص باستخدام واجهة `IMiddleware`
- ✅ **Request Pipeline** - فهم خط أنابيب middleware في ASP.NET Core
- ✅ **ترتيب Middleware** - الأهمية الحرجة لترتيب تسجيل middleware
- ✅ **تسجيل Request/Response** - قراءة وتسجيل بيانات طلبات واستجابات HTTP
- ✅ **مراقبة الأداء** - قياس وقت معالجة الطلبات باستخدام stopwatch
- ✅ **Response Headers** - إضافة headers مخصصة لاستجابات HTTP
- ✅ **أنماط جاهزة للإنتاج** - بناء مكونات middleware قابلة لإعادة الاستخدام

## 🚀 ما نبنيه

**نظام Custom Middleware جاهز للإنتاج** يتميز بثلاثة مكونات middleware متخصصة:

1. **RequestLoggingMiddleware** - تسجيل تفاصيل الطلبات الواردة وحالة الاستجابات الصادرة
2. **ResponseTimingMiddleware** - قياس وقت معالجة الطلبات وإضافة header `X-Response-Time`
3. **RequestResponseLoggingMiddleware** - تسجيل مفصل لمحتوى الطلبات والاستجابات للتصحيح

## 📁 هيكل المشروع

```
CustomMiddlewareApi/
├── Controllers/
│   └── ProductsController.cs        # نقاط نهاية API
├── Middleware/                       # ⭐ مكونات middleware مخصصة
│   ├── RequestLoggingMiddleware.cs  # تسجيل طلبات واستجابات HTTP
│   ├── ResponseTimingMiddleware.cs  # قياس وتسجيل وقت الاستجابة
│   └── RequestResponseLoggingMiddleware.cs # تسجيل مفصل للمحتوى
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
│       ├── CreateProductDto.cs      # طلب إنشاء المنتج
│       ├── UpdateProductDto.cs      # طلب تحديث المنتج
│       └── ProductResponseDto.cs    # استجابة المنتج مع حالة المخزون
├── Repositories/
│   ├── IProductRepository.cs        # واجهة المستودع
│   └── ProductRepository.cs         # تنفيذ المستودع
├── Services/
│   ├── IProductService.cs           # واجهة الخدمة
│   ├── ProductService.cs            # الخدمة مع منطق التحقق
│   ├── INotificationService.cs      # واجهة خدمة الإشعارات
│   └── NotificationService.cs       # تنفيذ الإشعارات
├── Data/
│   └── ProductsData.cs              # مخزن بيانات في الذاكرة
├── Mappings/
│   └── MappingProfile.cs            # تكوين AutoMapper
├── Program.cs                       # تكوين خط أنابيب middleware ⭐
└── CustomMiddlewareApi.http         # طلبات HTTP للاختبار
```

## 🏗️ بنية خط أنابيب Middleware

### **تدفق خط أنابيب الطلبات**

```
┌─────────────────────────────────────────────────────┐
│              طلب HTTP وارد                          │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      1. Exception Handler Middleware                │
│         • يغلف خط الأنابيب بالكامل                  │
│         • يلتقط الاستثناءات غير المعالجة            │
│         • يرجع RFC 7807 Problem Details             │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      2. Request/Response Logging Middleware         │
│         • يقرأ ويسجل محتوى الطلب                    │
│         • يفعل buffering للطلب                      │
│         • يلتقط محتوى الاستجابة                     │
│         • يسجل تفاصيل الطلب/الاستجابة كاملة         │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      3. Response Timing Middleware                  │
│         • يبدأ stopwatch                            │
│         • يضيف X-Response-Time header               │
│         • يسجل مدة المعالجة                         │
│         • ينبه إذا كان الطلب > 1000ms               │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│      4. Request Logging Middleware                  │
│         • يسجل طريقة ومسار الطلب                    │
│         • يسجل عنوان IP البعيد                      │
│         • يسجل كود حالة الاستجابة                   │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│            Middleware مدمج                          │
│         • Swagger (التطوير فقط)                     │
│         • HTTPS Redirection                         │
│         • Authorization                             │
│         • Endpoint Routing                          │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              Controllers/Endpoints                  │
│         • ProductsController                        │
│         • استدعاء الخدمات                           │
│         • إرجاع الاستجابات                          │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
      الاستجابة ترجع عبر middleware
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              استجابة HTTP للعميل                   │
│         • كود الحالة                                │
│         • Headers (تتضمن X-Response-Time)           │
│         • محتوى JSON                                │
└─────────────────────────────────────────────────────┘
```

## 💻 تفاصيل تنفيذ Middleware

### **1. RequestLoggingMiddleware** - تسجيل أساسي للطلبات والاستجابات

```csharp
public class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // قبل الطلب
        logger.LogInformation(
            "Incoming Request: {Method} {Path} from {RemoteIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        // استدعاء middleware التالي في خط الأنابيب
        await next(context);

        // بعد الاستجابة
        logger.LogInformation(
            "Outgoing Response: {StatusCode} for {Method} {Path}",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path);
    }
}
```

**المفاهيم الأساسية:**
- ينفذ واجهة `IMiddleware` لدعم حقن التبعية
- يستخدم **primary constructor** (ميزة C# 12) للكود الأنظف
- يسجل **قبل** استدعاء `next()` للطلب الوارد
- يسجل **بعد** استدعاء `next()` للاستجابة الصادرة
- يلتقط **RemoteIpAddress** لتتبع طلبات العميل

---

### **2. ResponseTimingMiddleware** - مراقبة الأداء

```csharp
public class ResponseTimingMiddleware(ILogger<ResponseTimingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // بدء stopwatch
        var stopwatch = Stopwatch.StartNew();

        // ربط OnStarting لإضافة header قبل إرسال الاستجابة
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            context.Response.Headers.Append("X-Response-Time", $"{elapsedMilliseconds}ms");
            return Task.CompletedTask;
        });

        // تنفيذ middleware التالي
        await next(context);

        // إيقاف stopwatch (إن لم يتم إيقافه بالفعل)
        stopwatch.Stop();

        // تسجيل الوقت المنقضي
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        logger.LogInformation(
            "Request {Method} {Path} completed in {ElapsedMilliseconds}ms with status {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            elapsedMilliseconds,
            context.Response.StatusCode);

        // تحذير إذا استغرق الطلب وقتاً طويلاً
        if (elapsedMilliseconds > 1000)
        {
            logger.LogWarning(
                "SLOW REQUEST: {Method} {Path} took {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds);
        }
    }
}
```

**المفاهيم الأساسية:**
- يستخدم `Stopwatch` لقياس الوقت بدقة
- **OnStarting callback** - يضمن إضافة header قبل بدء الاستجابة
- يضيف **X-Response-Time** header مخصص لكل استجابة
- يسجل مقاييس الأداء مع التسجيل المنظم
- **كشف الطلبات البطيئة** - ينبه إذا كان وقت الاستجابة > 1000ms
- مثالي لتحديد اختناقات الأداء

---

### **3. RequestResponseLoggingMiddleware** - تسجيل مفصل للمحتوى

```csharp
public class RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // تسجيل الطلب
        await LogRequest(context);

        // نسخ تدفق الاستجابة الأصلي
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        // تنفيذ middleware التالي
        await next(context);

        // تسجيل الاستجابة
        await LogResponse(context);

        // نسخ محتويات تدفق الاستجابة الجديد إلى التدفق الأصلي
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        logger.LogInformation(
            "HTTP Request Information:\n" +
            "Method: {Method}\n" +
            "Path: {Path}\n" +
            "QueryString: {QueryString}\n" +
            "Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            body);
    }

    private async Task LogResponse(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        logger.LogInformation(
            "HTTP Response Information:\n" +
            "StatusCode: {StatusCode}\n" +
            "Body: {Body}",
            context.Response.StatusCode,
            body);
    }
}
```

**المفاهيم الأساسية:**
- **EnableBuffering()** - يسمح بقراءة محتوى الطلب عدة مرات
- **Stream replacement** - يلتقط محتوى الاستجابة دون كسر الاستجابة
- يسجل محتوى الطلب الكامل (الطريقة، المسار، الاستعلام، المحتوى)
- يسجل محتوى الاستجابة الكامل (كود الحالة، المحتوى)
- **مهم**: إعادة تعيين موضع التدفق بعد القراءة لتجنب فقدان البيانات
- **استخدام بحذر** - يمكن أن يكون مطولاً في الإنتاج، فعّله فقط للتصحيح

---

## 🔌 تكوين Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// إضافة الخدمات
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// تسجيل معالجات الاستثناءات
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// تسجيل الخدمات لحقن التبعية
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

// ⭐ تسجيل خدمات middleware (مطلوب لواجهة IMiddleware)
builder.Services.AddScoped<RequestResponseLoggingMiddleware>();
builder.Services.AddScoped<ResponseTimingMiddleware>();
builder.Services.AddScoped<RequestLoggingMiddleware>();

var app = builder.Build();

// *** ترتيب MIDDLEWARE مهم! ***

// 1. Exception Handler - يغلف خط الأنابيب بالكامل لالتقاط جميع الاستثناءات
app.UseExceptionHandler();

// 2. Custom Middleware - Request/Response Logging (اختياري - قد يكون مطولاً)
// أزل التعليق لتفعيل تسجيل مفصل لمحتوى الطلب/الاستجابة
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// 3. Custom Middleware - Response Timing
// يقيس كم من الوقت يستغرق كل طلب ويضيف X-Response-Time header
app.UseMiddleware<ResponseTimingMiddleware>();

// 4. Custom Middleware - Request Logging
// يسجل معلومات الطلب الأساسية (الطريقة، المسار، IP)
app.UseMiddleware<RequestLoggingMiddleware>();

// 5. Swagger (التطوير فقط)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. HTTPS Redirection
app.UseHttpsRedirection();

// 7. Authorization
app.UseAuthorization();

// 8. Endpoint Routing - ربط المتحكمات
app.MapControllers();

app.Run();
```

**النقاط الحرجة:**
- **تسجيل IMiddleware**: يجب تسجيل فئات middleware في حاوي DI
- **ترتيب Middleware**: معالج الاستثناءات أولاً، ثم middleware مخصص، ثم المدمج
- **دورة حياة Scoped**: middleware الذي يستخدم `IMiddleware` يجب أن يكون scoped
- **الوعي بالبيئة**: Swagger فقط في التطوير

---

## 🧪 اختبار Middleware

### **1. اختبار Response Timing**
```http
GET https://localhost:7xxx/api/products
```

**تحقق من response headers** لـ:
```
X-Response-Time: 25ms
```

**تحقق من السجلات** لـ:
```
Request GET /api/products completed in 25ms with status 200
```

---

### **2. اختبار Request Logging**
```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "منتج تجريبي",
  "price": 99.99,
  "stockQuantity": 10
}
```

**تحقق من السجلات** لـ:
```
Incoming Request: POST /api/products from ::1
Outgoing Response: 201 for POST /api/products
```

---

### **3. اختبار تسجيل المحتوى المفصل**

فعّل `RequestResponseLoggingMiddleware` في Program.cs واختبر:

```http
POST https://localhost:7xxx/api/products
Content-Type: application/json

{
  "name": "Laptop Pro",
  "price": 1299.99,
  "stockQuantity": 5
}
```

**تحقق من السجلات** لمحتوى الطلب/الاستجابة الكامل:
```
HTTP Request Information:
Method: POST
Path: /api/products
QueryString:
Body: {"name":"Laptop Pro","price":1299.99,"stockQuantity":5}

HTTP Response Information:
StatusCode: 201
Body: {"id":4,"name":"Laptop Pro","price":1299.99,...}
```

---

### **4. اختبار تحذير الطلب البطيء**

نفّذ نقطة نهاية بطيئة وتحقق من التحذير:

```
SLOW REQUEST: GET /api/products/slow-endpoint took 1523ms
```

---

## 🎓 الفوائد الرئيسية

### **1. فصل الاهتمامات**
- ✅ Middleware يتعامل مع الاهتمامات المتقاطعة (التسجيل، التوقيت، headers)
- ✅ المتحكمات تركز على منطق الأعمال
- ✅ قاعدة كود نظيفة وقابلة للصيانة

### **2. قابلية إعادة الاستخدام**
- ✅ مكونات Middleware تعمل عبر جميع النقاط النهائية
- ✅ لا تكرار للكود في المتحكمات
- ✅ سهل التفعيل/التعطيل للميزات

### **3. مراقبة الأداء**
- ✅ تتبع تلقائي لوقت الاستجابة
- ✅ كشف الطلبات البطيئة
- ✅ تحديد اختناقات الأداء

### **4. التصحيح**
- ✅ تسجيل كامل للطلب/الاستجابة
- ✅ تتبع عنوان IP
- ✅ تسجيل منظم مع الارتباط

### **5. فوائد واجهة IMiddleware**
- ✅ دعم حقن التبعية
- ✅ خدمات Scoped (logger، DbContext، إلخ.)
- ✅ كود أنظف وقابل للاختبار
- ✅ إدارة دورة الحياة بواسطة حاوي DI

---

## 🔧 تشغيل المشروع

```bash
cd 10-custom-middleware/CustomMiddlewareApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`
**Products API**: `https://localhost:7xxx/api/products`

---

## 🎯 النقاط الرئيسية

1. **واجهة IMiddleware**: النهج المفضل لـ middleware مع دعم DI
2. **Primary Constructors**: استخدم ميزة C# 12 لحقن constructor أنظف
3. **ترتيب Middleware**: حرج - معالج الاستثناءات أولاً، routing أخيراً
4. **مراقبة الأداء**: استخدم Stopwatch و OnStarting callback للـ headers
5. **Request Buffering**: EnableBuffering() يسمح بقراءة محتوى الطلب عدة مرات
6. **إدارة Stream**: استبدل stream الاستجابة لالتقاط المحتوى دون كسر الاستجابة
7. **الوعي بالبيئة**: فعّل التسجيل المطول فقط في التطوير
8. **التسجيل المنظم**: استخدم placeholders مسماة لبحث أفضل في السجلات

---

## ➡️ الخطوة التالية

**قم بتوسيع نظام middleware هذا بـ:**
- **Authentication Middleware** - التحقق من API key أو JWT
- **Rate Limiting Middleware** - تحديد الطلبات لكل IP/مستخدم
- **Caching Middleware** - تخزين الاستجابات مؤقتاً لتحسين الأداء
- **Compression Middleware** - ضغط الاستجابات لتقليل النطاق الترددي
- **CORS Middleware** - التعامل مع طلبات cross-origin
- **Short-Circuit Middleware** - الإرجاع مبكراً بناءً على الشروط
- **Conditional Middleware** - تفعيل middleware بناءً على البيئة/التكوين

---

**💡 نصيحة احترافية**: ترتيب Middleware حرج! فكر دائماً في تدفق الطلب/الاستجابة. معالجات الاستثناءات تغلف كل شيء، التسجيل يحدث مبكراً، و routing يأتي أخيراً قبل النقاط النهائية!

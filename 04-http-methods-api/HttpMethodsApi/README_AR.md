# الفصل 04: HTTP Methods API - عمليات CRUD الكاملة

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![REST](https://img.shields.io/badge/REST-API-009688?style=flat-square)

## 📺 فيديو يوتيوب
**🔗 [شاهد الفصل 04: HTTP Methods API](https://www.youtube.com/watch?v=3pkXQIpd-Tk&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=6)**

## 🎯 أهداف التعلم

في نهاية هذا الفصل، ستتقن:
- ✅ جميع HTTP methods: GET, POST, PUT, PATCH, DELETE
- ✅ مبادئ تصميم RESTful API
- ✅ الاستخدام الصحيح لرموز حالة HTTP
- ✅ أنماط معالجة الطلب/الاستجابة
- ✅ معالجة الأخطاء والتسجيل

## 🚀 ما سنبنيه

API إدارة المنتجات الكامل مع عمليات CRUD كاملة:
- **إ**نشاء المنتجات (POST)
- **ق**راءة المنتجات (GET)
- **ت**حديث المنتجات (PUT & PATCH)
- **ح**ذف المنتجات (DELETE)

## 📁 هيكل المشروع

```
HttpMethodsApi/
├── Controllers/
│   └── ProductsController.cs    # كونترولر CRUD كامل
├── Models/
│   └── Product.cs              # نموذج كيان المنتج
├── Program.cs                  # تكوين Swagger + التسجيل
├── HttpMethodsApi.http         # أمثلة جميع HTTP methods
└── [ملفات ASP.NET Core المعيارية]
```

## 🛠️ نقاط نهاية API

| الطريقة | النقطة النهائية | الوصف | رموز الحالة |
|--------|----------|-------------|--------------|
| **GET** | `/api/products` | الحصول على جميع المنتجات | 200 OK |
| **GET** | `/api/products/{id}` | الحصول على منتج بالمعرف | 200 OK, 404 Not Found |
| **POST** | `/api/products` | إنشاء منتج جديد | 201 Created, 400 Bad Request |
| **PUT** | `/api/products/{id}` | تحديث المنتج كاملاً | 200 OK, 400 Bad Request, 404 Not Found |
| **PATCH** | `/api/products/{id}` | تحديث جزئي | 200 OK, 400 Bad Request, 404 Not Found |
| **DELETE** | `/api/products/{id}` | حذف المنتج | 204 No Content, 404 Not Found |

## 💻 أمثلة الكود

### **GET - استرداد جميع المنتجات**
```csharp
[HttpGet]
[ProducesResponseType(StatusCodes.Status200OK)]
public ActionResult<IEnumerable<Product>> GetProduct()
{
    logger.LogInformation("Getting all Products");
    return Ok(products);
}
```

### **POST - إنشاء منتج جديد**
```csharp
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public ActionResult<Product> CreateProduct(Product product)
{
    if (string.IsNullOrEmpty(product.Name))
        return BadRequest("Product name cannot be empty");

    product.Id = products.Max(x => x.Id) + 1;
    product.CreatedAt = DateTime.UtcNow;
    products.Add(product);

    Response.Headers.Append("entity-id", product.Id.ToString());
    return Created();
}
```

### **PUT مقابل PATCH - فهم الفرق**

**PUT (الاستبدال الكامل)**
```csharp
[HttpPut("{id}")]
public ActionResult<Product> UpdateProduct(int id, Product product)
{
    // يستبدل جميع خصائص المنتج الموجود
    existingProduct.Name = product.Name;
    existingProduct.Price = product.Price;
    existingProduct.Category = product.Category;
}
```

**PATCH (التحديث الجزئي)**
```csharp
[HttpPatch("{id}")]
public ActionResult<Product> PatchProduct(int id, Product product)
{
    // يحدث فقط الخصائص غير NULL/غير الافتراضية
    if (!string.IsNullOrEmpty(product.Name))
        existingProduct.Name = product.Name;
    
    if (product.Price > 0)
        existingProduct.Price = product.Price;
}
```

## 🔍 HTTP Methods - غوص عميق

### **GET - آمن وIdempotent**
- **الغرض**: استرداد البيانات بدون تأثيرات جانبية
- **الخصائص**: آمن، idempotent، قابل للتخزين المؤقت
- **حالات الاستخدام**: الحصول على عناصر مفردة أو مجموعات

### **POST - ليس آمناً ولا Idempotent**
- **الغرض**: إنشاء موارد جديدة
- **الخصائص**: ليس آمناً، ليس idempotent
- **الاستجابة**: 201 Created مع رأس Location

### **PUT - Idempotent لكن ليس آمناً**
- **الغرض**: استبدال المورد كاملاً
- **الخصائص**: Idempotent، ليس آمناً
- **القاعدة**: أرسل جميع الحقول المطلوبة

### **PATCH - ليس آمناً ولا Idempotent**
- **الغرض**: تحديثات جزئية للمورد
- **الخصائص**: ليس آمناً، ليس idempotent
- **القاعدة**: أرسل فقط الحقول المراد تحديثها

### **DELETE - Idempotent لكن ليس آمناً**
- **الغرض**: إزالة الموارد
- **الخصائص**: Idempotent، ليس آمناً
- **الاستجابة**: 204 No Content عند النجاح

## 🧪 اختبار API

### **باستخدام ملف HTTP**
افتح `HttpMethodsApi.http` وشغل هذه الطلبات:

```http
### الحصول على جميع المنتجات
GET https://localhost:7185/api/products

### الحصول على منتج بالمعرف
GET https://localhost:7185/api/products/1

### إنشاء منتج جديد
POST https://localhost:7185/api/products
Content-Type: application/json

{
  "name": "Gaming Mouse",
  "price": 59.99,
  "category": "Gaming"
}

### تحديث المنتج كاملاً
PUT https://localhost:7185/api/products/1
Content-Type: application/json

{
  "name": "Updated Laptop",
  "price": 1299.99,
  "category": "Electronics"
}

### تحديث جزئي
PATCH https://localhost:7185/api/products/1
Content-Type: application/json

{
  "price": 899.99
}

### حذف منتج
DELETE https://localhost:7185/api/products/1
```

## 📊 أمثلة الاستجابات

### **استجابة GET ناجحة (200 OK)**
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "price": 999.99,
    "category": "Electronics",
    "createdAt": "2025-07-25T10:30:00Z"
  }
]
```

### **استجابة الإنشاء (201 Created)**
```
Status: 201 Created
Headers:
  entity-id: 5
  location: /api/products/5
```

### **استجابة خطأ (404 Not Found)**
```
Status: 404 Not Found
```

## 🎓 أفضل الممارسات الموضحة

1. **رموز حالة HTTP صحيحة** - كل طريقة تعيد رموزاً مناسبة
2. **تكامل التسجيل** - تتبع استخدام API والتصحيح
3. **التحقق من الإدخال** - تحقق أساسي قبل المعالجة
4. **رؤوس الاستجابة** - رؤوس مخصصة للموارد المنشأة
5. **توثيق XML** - توثيق API مهني

## 🔧 تشغيل المشروع

```bash
cd 04-http-methods-api/HttpMethodsApi
dotnet restore
dotnet run
```

**Swagger UI**: `https://localhost:7xxx/swagger`

## ➡️ الخطوات التالية

تريد جعل API الخاص بك أكثر احترافية مع التحقق المناسب و DTOs؟
**[الفصل 05: DTOs والتحقق](../05-dto-and-validations/)**

## 🤔 استكشاف الأخطاء وإصلاحها

**400 Bad Request في POST؟**
- تحقق من تنسيق JSON والحقول المطلوبة
- تأكد من ضبط رأس Content-Type إلى `application/json`

**404 Not Found؟**
- تحقق من وجود معرف المنتج
- تحقق من أن مسار URL يطابق نمط المسار

---

**💡 نصيحة احترافية**: استخدم PATCH للتحديثات الجزئية و PUT فقط عندما تريد استبدال المورد كاملاً!
# 🚀 سلسلة دروس ASP.NET Core 9 Web API الشاملة

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=for-the-badge&logo=dotnet)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![YouTube](https://img.shields.io/badge/YouTube-Tutorial-FF0000?style=for-the-badge&logo=youtube)

> **سلسلة دروس عملية شاملة لبناء واجهات برمجة التطبيقات الحديثة باستخدام ASP.NET Core 9**  
> من المبتدئ المطلق إلى تطبيقات جاهزة للإنتاج! 🎯

## 📺 سلسلة دروس يوتيوب

يحتوي هذا المستودع على جميع الأكواد المصدرية لسلسلة دروسنا الشاملة لـ ASP.NET Core 9 Web API. كل مجلد يمثل فصلاً كاملاً مع أمثلة أكواد عملية.

**🔗 [شاهد القائمة الكاملة على يوتيوب](#)** *(أضف رابط قائمة التشغيل هنا)*

## 📚 هيكل الدروس

| الفصل | الموضوع | المفاهيم الأساسية | المدة |
|---------|-------|--------------|----------|
| **01** | [المقدمة والإعداد](./01-introduction-and-setup/) | إنشاء المشروع، إعداد .NET 9 | ⏱️ ~15 دقيقة |
| **02-03** | [أول كونترولر و Swagger](./02-03-first-controller-and-swagger/) | الكونترولرز، توثيق API | ⏱️ ~25 دقيقة |
| **04** | [HTTP Methods API](./04-http-methods-api/) | GET, POST, PUT, PATCH, DELETE | ⏱️ ~35 دقيقة |
| **05** | [DTOs والتحقق](./05-dto-and-validations/) | كائنات نقل البيانات، ModelState | ⏱️ ~30 دقيقة |
| **06** | [تكامل AutoMapper](./06-automapper/) | ربط الكائنات، معمارية نظيفة | ⏱️ ~20 دقيقة |

## 🎯 ما ستتعلمه

### **مفاهيم ASP.NET Core الأساسية**
- ✅ إعداد مشاريع ASP.NET Core 9 من الصفر
- ✅ بناء واجهات برمجة تطبيقات RESTful مع رموز حالة HTTP صحيحة
- ✅ تنفيذ جميع عمليات CRUD (إنشاء، قراءة، تحديث، حذف)
- ✅ فهم نمط MVC في سياق Web API

### **توثيق وفحص API**
- ✅ تكامل Swagger/OpenAPI للتوثيق التلقائي
- ✅ تعليقات XML لتوثيق غني للـ API
- ✅ ملفات طلبات HTTP لفحص النقاط النهائية
- ✅ خصائص ProducesResponseType للعقود الواضحة

### **إدارة البيانات والتحقق**
- ✅ كائنات نقل البيانات (DTOs) لتصميم API نظيف
- ✅ التحقق من الإدخال باستخدام DataAnnotations
- ✅ معالجة الأخطاء ورسائل التحقق المخصصة
- ✅ AutoMapper لربط الكائنات بكفاءة

### **أفضل الممارسات**
- ✅ حقن التبعية والتسجيل
- ✅ فصل الاهتمامات مع هيكل مشروع صحيح
- ✅ دلالات HTTP methods (idempotency, safety)
- ✅ أنماط الاستجابة ورموز الحالة

## 🚀 البداية السريعة

### المتطلبات المسبقة
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)، [VS Code](https://code.visualstudio.com/)، أو [JetBrains Rider](https://www.jetbrains.com/rider/)

### تشغيل أي مشروع
1. **استنساخ المستودع**
   ```bash
   git clone https://github.com/yourusername/aspnet-core-9-web-api-complete-tutorial.git
   cd aspnet-core-9-web-api-complete-tutorial
   ```

2. **الانتقال إلى الفصل المرغوب**
   ```bash
   cd 04-http-methods-api/HttpMethodsApi
   ```

3. **استعادة التبعيات**
   ```bash
   dotnet restore
   ```

4. **تشغيل التطبيق**
   ```bash
   dotnet run
   ```

5. **فتح Swagger UI**
   - انتقل إلى: `https://localhost:7xxx/swagger`
   - اختبر جميع النقاط النهائية بشكل تفاعلي!

## 📖 تفاصيل الفصول

### **الفصل 01: المقدمة والإعداد**
- إنشاء أول مشروع ASP.NET Core 9 Web API
- فهم هيكل المشروع والملفات
- Program.cs ونموذج الاستضافة الدنيا

### **الفصل 02-03: أول كونترولر و Swagger**
- بناء أول كونترولر API
- إضافة Swagger لتوثيق API
- تعليقات XML للتوثيق الغني
- الفحص باستخدام Swagger UI

### **الفصل 04: HTTP Methods API**
- تنفيذ جميع HTTP methods (GET, POST, PUT, PATCH, DELETE)
- فهم مبادئ RESTful
- استخدام رموز الحالة الصحيحة
- معالجة الأخطاء والتسجيل

### **الفصل 05: DTOs والتحقق**
- كائنات نقل البيانات لتصميم API نظيف
- التحقق من الإدخال باستخدام DataAnnotations
- رسائل التحقق المخصصة
- فصل النماذج الداخلية عن عقود API

### **الفصل 06: تكامل AutoMapper**
- تثبيت وتكوين AutoMapper
- الربط بين الكيانات و DTOs
- تقليل كود الربط المكرر
- أنماط المعمارية النظيفة

## 🔧 هيكل المشروع

```
aspnet-core-9-web-api-tutorial/
├── 01-introduction-and-setup/
│   └── AspNetCore9WebApiIntro/
├── 02-03-first-controller-and-swagger/
│   └── FirstControllerAndSwagger/
├── 04-http-methods-api/
│   └── HttpMethodsApi/
├── 05-dto-and-validations/
│   └── DtoAndValidation/
├── 06-automapper/
│   └── AutoMapperApi/
└── README.md
```

## 🛠️ التقنيات المستخدمة

- **Framework**: ASP.NET Core 9.0
- **اللغة**: C# 12
- **التوثيق**: Swagger/OpenAPI 3.0
- **التحقق**: DataAnnotations
- **الربط**: AutoMapper
- **IDE**: JetBrains Rider / Visual Studio

## 📋 ميزات API المغطاة

### **API إدارة المنتجات**
- **GET** `/api/products` - استرداد جميع المنتجات
- **GET** `/api/products/{id}` - الحصول على منتج بالمعرف
- **POST** `/api/products` - إنشاء منتج جديد
- **PUT** `/api/products/{id}` - تحديث المنتج كاملاً
- **PATCH** `/api/products/{id}` - تحديث جزئي للمنتج
- **DELETE** `/api/products/{id}` - حذف المنتج

### **تنسيقات الاستجابة**
- استجابات JSON مع أنواع محتوى صحيحة
- معالجة أخطاء متسقة
- تفاصيل أخطاء التحقق
- رؤوس مخصصة للموارد المنشأة

## 🎓 مسار التعلم

**الترتيب المُوصى به للمبتدئين:**
1. ابدأ بـ **الفصل 01** لإعداد المشروع
2. اتبع **الفصول 02-03** لفهم الكونترولرز
3. أتقن **الفصل 04** لـ HTTP methods
4. تعلم **الفصل 05** للـ DTOs المهنية
5. اكمل بـ **الفصل 06** لـ AutoMapper

**للمطورين ذوي الخبرة:**
- انتقل لأي فصل حسب حاجتك
- كل مشروع مستقل وقابل للتشغيل

## 🤝 المساهمة

وجدت مشكلة أو تريد تحسين شيء؟ لا تتردد في:
- 🐛 الإبلاغ عن الأخطاء
- 💡 اقتراح التحسينات
- 📝 إصلاح التوثيق
- ⭐ إعطاء نجمة للمستودع إذا ساعدك!

## 📞 الدعم والمجتمع

- **تعليقات يوتيوب**: اطرح أسئلة تحت كل فيديو
- **GitHub Issues**: الإبلاغ عن المشاكل التقنية
- **المناقشات**: شارك مشاريعك والتحسينات

## 📜 الرخصة

هذا المشروع مرخص تحت رخصة MIT - انظر ملف [LICENSE](LICENSE) للتفاصيل.

## ➡️ ماذا بعد؟

🎉 **تهانينا!** لقد أكملت سلسلة دروس ASP.NET Core 9 Web API الأساسية!

**مسارات التعلم التالية:**
- **Entity Framework Core** لتكامل قاعدة البيانات
- **المصادقة والتفويض** لـ APIs آمنة
- **استراتيجيات التخزين المؤقت** لتحسين الأداء
- **إصدار API** لـ APIs متطورة
- **اختبار الوحدة** لكود موثوق

## 💡 نصائح احترافية AutoMapper

**AutoMapper يحذف الكود المتكرر للربط ويوفر:**
- **ربط تلقائي** بين DTOs والكيانات مع تكوين أدنى
- **حساب حالة المخزون** مع منطق ربط مخصص لقواعد العمل
- **كونترولرز أنظف** مع تقليل الكود المكرر - من 20+ سطر إلى 1-2 سطر لكل ربط
- **تكوين ربط مركزي** في الملفات الشخصية لصيانة أفضل
- **فوائد الأداء** من خلال الربط المجمع والتعبيرات المخزنة مؤقتاً

**الفوائد الرئيسية الموضحة في الفصل 06:**
- ✅ **تقليل 90%** في أسطر كود الربط
- ✅ **تكوين مركزي** في MappingProfile.cs
- ✅ **سيناريوهات متقدمة** مثل حساب حالة المخزون
- ✅ **فصل أفضل للاهتمامات** بين الكونترولرز ومنطق الربط

## 🙏 الشكر والتقدير

- Microsoft للمنصة الرائعة .NET
- فريق Swashbuckle لتكامل Swagger
- مساهمي AutoMapper
- مجتمع .NET الرائع

---

**⭐ لا تنس إعطاء نجمة لهذا المستودع إذا ساعدك في تعلم ASP.NET Core!**

**🔔 اشترك في قناتنا على يوتيوب للمزيد من دروس .NET!**
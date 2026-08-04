# الفصل 01: المقدمة وإعداد ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=flat-square&logo=dotnet)

## 📺 فيديو يوتيوب
**🔗 [شاهد الفصل 01: المقدمة والإعداد](https://www.youtube.com/watch?v=BEG49WICGEo&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=13)**

## 🎯 أهداف التعلم

في نهاية هذا الفصل، ستفهم:
- ✅ ما هو ASP.NET Core ولماذا نستخدم الإصدار 9
- ✅ كيفية إنشاء مشروع Web API جديد من الصفر
- ✅ فهم هيكل المشروع الافتراضي
- ✅ كيف يعمل نموذج الاستضافة الدنيا الجديد في .NET 9

## 🚀 ما سنبنيه

مشروع ASP.NET Core 9 Web API أساسي مع:
- تكوين استضافة دنيا
- إعداد بيئة تطوير افتراضية
- فهم هيكل المشروع الأساسي

## 📁 هيكل المشروع

```
AspNetCore9WebApiIntro/
├── Program.cs                  # نقطة دخول التطبيق
├── appsettings.json           # تكوين التطبيق
├── appsettings.Development.json # إعدادات التطوير
├── Properties/
│   └── launchSettings.json    # ملفات التشغيل/التصحيح
└── AspNetCore9WebApiIntro.csproj # ملف المشروع
```

## 🔧 شرح الملفات الأساسية

### **Program.cs**
قلب تطبيقك باستخدام نموذج الاستضافة الدنيا:
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Run();
```

### **appsettings.json**
تكوين تطبيقك:
- اتصالات قاعدة البيانات
- مفاتيح API
- إعدادات خاصة بالتطبيق

### **launchSettings.json**
تكوين بيئة التطوير:
- منافذ خادم التطوير
- متغيرات البيئة
- إعدادات إطلاق المتصفح

## 💻 تشغيل المشروع

1. **الانتقال إلى مجلد المشروع**
   ```bash
   cd 01-introduction-and-setup/AspNetCore9WebApiIntro
   ```

2. **استعادة حزم NuGet**
   ```bash
   dotnet restore
   ```

3. **تشغيل التطبيق**
   ```bash
   dotnet run
   ```

4. **اختبار النقطة النهائية**
   - افتح المتصفح: `https://localhost:7xxx`
   - يجب أن ترى: "Hello World!"

## 🔍 ما الجديد في .NET 9

- **أداء محسن** مع جمع قمامة أفضل
- **APIs دنيا محسنة** مع المزيد من الميزات
- **تجربة مطور أفضل** مع أدوات محسنة
- **تحسينات Native AOT** لأوقات بدء أسرع

## 📝 المتطلبات المسبقة

- فهم أساسي لبرمجة C#
- الإلمام بمفاهيم HTTP
- .NET 9 SDK مثبت
- IDE المفضل لديك (Visual Studio, VS Code, أو Rider)

## ➡️ الخطوات التالية

جاهز لإضافة وظائف حقيقية؟ تابع إلى:
**[الفصل 02-03: أول كونترولر و Swagger](../02-03-first-controller-and-swagger/)**

## 🤔 المشاكل الشائعة

**المنفذ مستخدم بالفعل؟**
- تحقق من `Properties/launchSettings.json` وغير أرقام المنافذ

**أخطاء البناء؟**
- تأكد من تثبيت .NET 9 SDK: `dotnet --version`
- شغل `dotnet clean` ثم `dotnet restore`

---

**💡 نصيحة**: هذا مجرد الأساس! السحر الحقيقي يحدث في الفصول التالية حيث نبني API منتجات كاملة.
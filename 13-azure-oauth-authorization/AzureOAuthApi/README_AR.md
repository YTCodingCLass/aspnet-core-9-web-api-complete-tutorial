# المصادقة والتفويض باستخدام Azure OAuth في ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Authentication](https://img.shields.io/badge/Authentication-Microsoft_Identity_Web-0078D4?style=flat-square)
![OAuth](https://img.shields.io/badge/OAuth_2.0-Authorization_Code_%2B_PKCE-EB5424?style=flat-square)

## 📺 فيديو يوتيوب

رابط فيديو التطبيق العملي للفصل 13 غير متوفر في المستودع حتى الآن.

**متطلب سابق:** [شرح OAuth 2.0 Authorization Code Flow مع PKCE باستخدام المخططات](https://www.youtube.com/watch?v=s7CHVYNX1C8&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=1)

> يشرح الفيديو المرتبط مفاهيم OAuth الخاصة بالفصل 12، وليس فيديو تطبيق Azure العملي للفصل 13.

## 🎯 أهداف التعلم

بنهاية هذا الفصل، ستفهم كيفية:

- إعداد ASP.NET Core Web API للتحقق من Access Tokens الصادرة عن Microsoft Entra ID.
- تسجيل JWT bearer authentication باستخدام `Microsoft.Identity.Web`.
- حماية Controller باستخدام `[Authorize]`.
- وصف OAuth 2.0 Authorization Code flow في Swagger/OpenAPI.
- استخدام PKCE من Swagger UI دون وضع Client Secret داخل المتصفح.
- إظهار النطاقين `Products.Read` و`Products.Write` في نافذة التفويض داخل Swagger.
- وضع `UseAuthentication()` قبل `UseAuthorization()` في مسار الطلب.
- إبقاء Tenant IDs وClient IDs والأسرار خارج التوثيق وإعدادات الإنتاج المتعقبة.

## 🚀 ما نبنيه

يؤمّن هذا الفصل Products API الموروث باستخدام Microsoft Entra ID:

1. يتحقق API من bearer tokens باستخدام قسم الإعدادات `AzureAd`.
2. تتطلب جميع Actions في `ProductsController` مستخدماً جرت مصادقته.
3. يبدأ Swagger UI تدفق OAuth 2.0 Authorization Code ويستخدم PKCE.
4. يعرض Swagger الصلاحيتين `Products.Read` و`Products.Write`.
5. تحصل الطلبات التي لا تحمل Access Token صالحاً على `401 Unauthorized`.

أما أكواد Repository وService وDTO وAutoMapper ومعالجة الاستثناءات وCustom Middleware والبيانات داخل الذاكرة وOptions Pattern فهي موروثة من الفصل 11.

## 📁 هيكل المشروع

```text
AzureOAuthApi/
├── Controllers/
│   └── ProductsController.cs          # ⭐ يحمي [Authorize] جميع Actions
├── Configuration/
│   └── RequestResponseLoggingOptions.cs # Options Pattern موروث
├── Data/
│   └── InMemoryDatabase.cs            # بيانات موروثة داخل الذاكرة
├── Exceptions/                        # استثناءات مخصصة موروثة
├── Handlers/                          # معالجات استثناءات موروثة
├── Mappings/
│   └── MappingProfile.cs              # AutoMapper profile موروث
├── Middleware/                        # Custom middleware موروث
├── Models/                            # Models وDTOs الخاصة بالمنتجات
├── Repositories/                      # طبقة Repository موروثة
├── Services/                          # طبقة Service موروثة
├── Properties/
│   └── launchSettings.json            # Launch profiles محلية
├── AzureOAuthApi.csproj               # ⭐ حزم Identity ودعم user secrets
├── Program.cs                         # ⭐ المصادقة والتفويض وSwagger OAuth
├── appsettings.json                   # إعدادات مشتركة غير حساسة
└── appsettings.Development.json       # ⭐ بنية إعدادات AzureAd
```

> لا يحتوي هذا الفصل على ملف طلبات `.http`.

## 🆕 ما الجديد مقارنة بالفصل 11

الفصل 11 (`ConfigurationOptionsApi`) هو آخر مشروع قابل للتشغيل قبل هذا الفصل. يحتوي الفصل 12 على مخططات ونظرية OAuth فقط، ولذلك نستخدمه كسياق مفاهيمي لا كأساس لمقارنة الكود.

| الجانب | الفصل 11 | الفصل 13 |
|---|---|---|
| Authentication | غير معدّة | JWT bearer authentication عبر `Microsoft.Identity.Web` |
| خدمات Authorization | استدعاء Middleware موجود فقط | تسجيل `AddAuthorization()` بشكل صريح |
| حماية Controller | نقاط نهاية المنتجات متاحة دون مصادقة | يحمي `[Authorize]` الـController بالكامل |
| Swagger | Swagger UI أساسي | OAuth 2.0 Authorization Code flow مع PKCE |
| Scopes التي يعرضها Swagger | لا يوجد | `Products.Read` و`Products.Write` |
| مسار Middleware | `UseAuthorization()` | `UseAuthentication()` قبل `UseAuthorization()` |
| إعدادات Identity | لا يوجد | قسم `AzureAd` في إعدادات Development |
| البنية الحالية | Repository وService وMiddleware والاستثناءات وOptions Pattern | موروثة دون إضافة طبقة معمارية جديدة |

## 🏗️ تدفق طلب المصادقة

```text
User
  │
  │ 1. يضغط Authorize في Swagger UI
  ▼
Microsoft Entra ID /authorize endpoint
  │
  │ 2. يعود Authorization code إلى Swagger UI
  ▼
Swagger UI + PKCE
  │
  │ 3. يستبدل Code وVerifier عبر /token
  │ 4. يرسل Authorization: Bearer <access_token>
  ▼
UseAuthentication()
  │  يتحقق من Token وينشئ المستخدم الذي جرت مصادقته
  ▼
UseAuthorization()
  │  يقيّم [Authorize]
  ▼
ProductsController → ProductService → ProductRepository → InMemoryDatabase
```

تجيب Authentication عن سؤال «من المتصل؟»، بينما تجيب Authorization عن سؤال «هل يُسمح لهذا المتصل بالوصول إلى Endpoint؟». نحتاج إلى الاثنتين، كما أن ترتيب Middleware مهم.

## 💻 التطبيق خطوة بخطوة

### الخطوة 1: إضافة حزم Identity

يستهدف `AzureOAuthApi.csproj` الإصدار .NET 9 ويتضمن الحزم الخاصة بهذا الفصل:

```xml
<PackageReference Include="Azure.Extensions.AspNetCore.Configuration.Secrets" Version="1.5.0" />
<PackageReference Include="Azure.Identity" Version="1.21.0" />
<PackageReference Include="Microsoft.Identity.Web" Version="4.7.0" />
```

تُستخدم `Microsoft.Identity.Web` فعلياً لإعداد التحقق من bearer token. أما حزمتا Azure فمثبتتان، لكن `Program.cs` الحالي لا يربط التطبيق بـAzure Key Vault.

يحتوي المشروع أيضاً على `UserSecretsId` لتفعيل التخزين المحلي عبر user secrets، وقد حُذفت قيمته الخاصة بالجهاز من هذا التوثيق عمداً.

### الخطوة 2: تعريف بنية إعدادات `AzureAd`

يقرأ التطبيق الحالي إعدادات Identity من `AzureAd`. استخدم قيماً بديلة واضحة في التوثيق وSource Control المشتركين:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "",
    "Scopes": "Products.Read Products.Write"
  }
}
```

- يحدد `TenantId` مستأجر Microsoft Entra.
- يحدد `ClientId` تسجيل التطبيق المستخدم في هذا الدرس.
- يجب إنشاء `Products.Read` و`Products.Write` ضمن **Expose an API** في Microsoft Entra ID.
- يبني الكود الحالي عناوين Scopes بالشكل `api://YOUR_CLIENT_ID/Products.Read` و`api://YOUR_CLIENT_ID/Products.Write`.
- يستخدم Swagger تدفق Authorization Code مع PKCE، لذلك لا تضع Client Secret داخل Swagger UI أو JSON متعقب.

### الخطوة 3: تسجيل Authentication وAuthorization

يحدد `Program.cs` مخطط JWT bearer كمخطط Authentication افتراضي، ويربط Microsoft Identity Web بالإعدادات:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddAuthorization();
```

يقرأ `AddMicrosoftIdentityWebApi` قسم `AzureAd` الافتراضي ويعد التحقق من Access Token داخل API.

### الخطوة 4: إعداد OAuth 2.0 Scopes في Swagger

يقرأ Swagger معرفي Tenant وClient، ثم يبني Endpoints الخاصة بمنصة Microsoft Identity بالإصدار v2.0:

```csharp
var tenantId = builder.Configuration["AzureAd:TenantId"];
var clientId = builder.Configuration["AzureAd:ClientId"];

options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
{
    Type = SecuritySchemeType.OAuth2,
    Flows = new OpenApiOAuthFlows
    {
        AuthorizationCode = new OpenApiOAuthFlow
        {
            AuthorizationUrl = new Uri(
                $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
            TokenUrl = new Uri(
                $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
            Scopes = new Dictionary<string, string>
            {
                { $"api://{clientId}/Products.Read", "Read products" },
                { $"api://{clientId}/Products.Write", "Write products" }
            }
        }
    }
});
```

يطلب OpenAPI security requirement النطاق `Products.Read` افتراضياً. بعد ذلك يمرر Swagger UI قيمة Client ID المعدة ويفعّل PKCE:

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
    options.OAuthUsePkce();
    options.OAuthScopeSeparator(" ");
});
```

سجّل Swagger redirect URI الذي يطابق Profile المستخدم، مثلاً:

```text
http://localhost:5213/swagger/oauth2-redirect.html
```

### الخطوة 5: حماية Products Controller

تُطبّق `[Authorize]` على مستوى Controller، ولذلك تشمل GET وPOST وPUT وDELETE وbulk-create:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(IProductService productService) : ControllerBase
{
    // All actions require an authenticated user.
}
```

مهم: يتحقق الدرس الحالي من وجود مستخدم جرت مصادقته فقط. يعرض Swagger النطاقين `Products.Read` و`Products.Write`، لكن الكود لا يستدعي `RequiredScope` ولا يعرّف Authorization policies ولا يطبق متطلبات Scopes مختلفة على Actions القراءة والكتابة. لذلك يستطيع Access Token صالح لمستخدم جرت مصادقته الوصول إلى جميع Actions في Controller. ويُعد فرض الصلاحيات الدقيقة حسب Scope خطوة منطقية تالية.

### الخطوة 6: وضع Authentication قبل Authorization

يحافظ مسار الطلب على Middleware الخاصة بالاستثناءات والتسجيل الموروثة، ثم يضيف Middleware الأمنية بهذا الترتيب:

```csharp
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
```

يجب تشغيل `UseAuthentication()` أولاً حتى يتحقق من bearer token ويملأ `HttpContext.User` قبل أن تقيّم Authorization السمة `[Authorize]`.

## 🔐 قائمة إعداد Microsoft Entra ID

قبل الاختبار عبر Swagger:

1. أنشئ App Registration للـAPI أو اختر واحداً موجوداً.
2. ضمن **Expose an API**، عيّن Application ID URI إلى `api://YOUR_CLIENT_ID`.
3. أنشئ Delegated Scopes باسم `Products.Read` و`Products.Write`.
4. أضف Platform redirect URI يطابق Swagger، مثل `http://localhost:5213/swagger/oauth2-redirect.html`.
5. امنح Client الأذونات اللازمة للـScopes المكشوفة، وقدّم Consent إذا كان Tenant يتطلبه.
6. مرّر Tenant ID وClient ID الصحيحين محلياً دون تثبيت المعرفات الحقيقية أو الأسرار في المستودع.

يجب أن يتطابق App Registration مع عناوين Scopes وredirect URI الدقيقة التي يبنيها الكود.

## 🔒 إعداد محلي آمن

استخدم user secrets في التطوير المحلي بدلاً من تثبيت المعرفات الحقيقية داخل الملفات المتعقبة:

```bash
cd 13-azure-oauth-authorization/AzureOAuthApi
dotnet user-secrets set "AzureAd:TenantId" "YOUR_TENANT_ID"
dotnet user-secrets set "AzureAd:ClientId" "YOUR_CLIENT_ID"
```

تُعد Environment Variables مصدراً مدعوماً آخر للإعدادات:

```bash
AzureAd__TenantId=YOUR_TENANT_ID
AzureAd__ClientId=YOUR_CLIENT_ID
```

ملاحظات أمنية:

- لا تثبّت Tenant IDs أو Client IDs أو Client Secrets أو Tokens أو أسماء Vault الحقيقية في توثيق الدرس.
- يحمي PKCE عملية استبدال Authorization Code، لكنه لا يجعل المتصفح مكاناً آمناً لتخزين Client Secret.
- تجنب تسجيل bearer tokens أو Authorization headers الحساسة.
- على الرغم من وجود Dependencies الخاصة بـAzure Key Vault، فلن يصبح Key Vault فعالاً حتى يُضاف كود الإعداد بصورة مقصودة.

## 🧪 اختبار API المحمي باستخدام Swagger

### 1. تشغيل Development profile

يتوفر Swagger في بيئة Development فقط. يستخدم Profile المسمى `http` بيئة Development ويستمع على المنفذ `5213`:

```bash
cd 13-azure-oauth-authorization/AzureOAuthApi
dotnet run --launch-profile http
```

افتح:

```text
http://localhost:5213/swagger
```

يضبط Launch profile المسمى `https` حالياً قيمة `ASPNETCORE_ENVIRONMENT` إلى `Production`، لذلك لا يظهر Swagger عند استخدامه.

### 2. التأكد من رفض الوصول دون مصادقة

استدعِ `GET /api/Products` قبل تنفيذ Authorize.

النتيجة المتوقعة:

```http
HTTP/1.1 401 Unauthorized
```

### 3. تنفيذ Authorization عبر Microsoft Entra ID

1. اضغط **Authorize** في Swagger UI.
2. اختر Scope أو Scopes المتاحة.
3. سجّل الدخول بحساب مسموح له داخل Tenant.
4. أكمل Consent إذا طُلب.
5. استدعِ `GET /api/Products` مرة أخرى.

عند استخدام Access Token صالح لهذا API، تعيد Endpoint النتيجة `200 OK` وبيانات المنتجات المخزنة في الذاكرة. يمنع Token منتهي الصلاحية أو تالف، أو Tenant خاطئ، أو Audience خاطئ، أو عدم تطابق Scope URI أو redirect URI نجاح الاختبار.

## ▶️ البناء والتشغيل

```bash
cd 13-azure-oauth-authorization/AzureOAuthApi
dotnet restore
dotnet build
dotnet run --launch-profile http
```

يستهدف المشروع .NET 9، لذلك ثبّت .NET 9 SDK قبل تشغيل هذه الأوامر.

## 🛠️ استكشاف الأخطاء وإصلاحها

| العَرَض | ما يجب التحقق منه |
|---|---|
| Swagger غير ظاهر | شغّل Profile المسمى `http` أواضبط البيئة إلى Development بطريقة أخرى |
| ظهور `401 Unauthorized` بعد تسجيل الدخول | تحقق من Tenant وAudience/Client ID وانتهاء Token، ومن إرسال Swagger للـbearer token |
| خطأ redirect من نوع `AADSTS50011` | سجّل Swagger OAuth redirect URI الدقيق |
| Scope غير ظاهر | اكشف `Products.Read` و`Products.Write` باستخدام Application ID URI المطابق تماماً |
| نجاح Authorization مع بقاء عمليات الكتابة متاحة | يستخدم هذا الفصل `[Authorize]` فقط، ولم يُطبق فرض Scope حسب Action بعد |
| إعدادات Key Vault بلا تأثير | الحزم مثبتة، لكن التطبيق الحالي لا يضيف Key Vault كمصدر إعدادات |

## ✅ الخلاصات الرئيسية

- تدمج `Microsoft.Identity.Web` التحقق من Tokens الصادرة عن Microsoft Entra ID مع JWT bearer authentication في ASP.NET Core.
- تجعل `[Authorize]` كل Actions داخل `ProductsController` تتطلب مستخدماً جرت مصادقته.
- يستطيع Swagger توضيح Authorization Code flow بصورة آمنة باستخدام PKCE.
- عرض Scopes في Swagger لا يفرض الصلاحيات وحده.
- يجب أن يسبق `UseAuthentication()` الاستدعاء `UseAuthorization()`.
- يحتفظ الفصل 13 ببنية تطبيق الفصل 11 ويضيف حولها حداً أمنياً.
- تنتمي المعرفات الحساسة إلى user secrets أو Environment Variables أو إعدادات Cloud مُدارة أو Secret Store أُعد بصورة مقصودة، وليس إلى ملفات الدرس المتعقبة.

## 🚀 الخطوات التالية

- فرض `Products.Read` على GET Actions و`Products.Write` على Actions التي تغيّر البيانات.
- إضافة Role-based أو Policy-based authorization عندما يتطلب Domain ذلك.
- إضافة Integration Tests آلية للطلبات دون مصادقة، ولـTokens غير الصالحة، وللطلبات المصرح بها.
- إعداد Azure Key Vault فقط عندما يقدمه الدرس عمداً وتتوفر استراتيجية Credentials مناسبة.

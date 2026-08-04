# Azure OAuth Authorization in ASP.NET Core 9

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Authentication](https://img.shields.io/badge/Authentication-Microsoft_Identity_Web-0078D4?style=flat-square)
![OAuth](https://img.shields.io/badge/OAuth_2.0-Authorization_Code_%2B_PKCE-EB5424?style=flat-square)

## 📺 YouTube Video

**🔗 [Watch Chapter 13: Azure AD OAuth 2.0](https://www.youtube.com/watch?v=rC2Yx56p1dg&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=1)**

**Prerequisite:** [OAuth 2.0 Authorization Code Flow with PKCE — diagram explanation](https://www.youtube.com/watch?v=s7CHVYNX1C8&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=2)

> Watch the chapter 12 diagram lesson first for the OAuth concepts, then continue with the chapter 13 Azure implementation video.

## 🎯 Learning Objectives

By the end of this chapter, you will understand how to:

- Configure an ASP.NET Core Web API to validate Microsoft Entra ID access tokens.
- Register JWT bearer authentication with `Microsoft.Identity.Web`.
- Protect a controller with `[Authorize]`.
- Describe OAuth 2.0 Authorization Code flow in Swagger/OpenAPI.
- Use PKCE from Swagger UI without placing a client secret in the browser.
- Expose `Products.Read` and `Products.Write` scopes in the Swagger authorization dialog.
- Place `UseAuthentication()` before `UseAuthorization()` in the request pipeline.
- Keep tenant IDs, client IDs, and secrets out of committed documentation and production configuration.

## 🚀 What We Build

This chapter secures the inherited products API with Microsoft Entra ID:

1. The API validates bearer tokens using the `AzureAd` configuration section.
2. Every action in `ProductsController` requires an authenticated caller.
3. Swagger UI starts the OAuth 2.0 Authorization Code flow and uses PKCE.
4. Swagger advertises `Products.Read` and `Products.Write` permissions.
5. Requests without a valid access token receive `401 Unauthorized`.

The repository, service, DTO, AutoMapper, exception-handler, custom-middleware, in-memory data, and Options Pattern code are inherited from chapter 11.

## 📁 Project Structure

```text
AzureOAuthApi/
├── Controllers/
│   └── ProductsController.cs          # ⭐ [Authorize] protects every action
├── Configuration/
│   └── RequestResponseLoggingOptions.cs # Inherited Options Pattern
├── Data/
│   └── InMemoryDatabase.cs            # Inherited in-memory data
├── Exceptions/                        # Inherited custom exceptions
├── Handlers/                          # Inherited exception handlers
├── Mappings/
│   └── MappingProfile.cs              # Inherited AutoMapper profile
├── Middleware/                        # Inherited custom middleware
├── Models/                            # Product models and DTOs
├── Repositories/                      # Inherited repository layer
├── Services/                          # Inherited service layer
├── Properties/
│   └── launchSettings.json            # Local launch profiles
├── AzureOAuthApi.csproj               # ⭐ Identity packages and user-secrets support
├── Program.cs                         # ⭐ Authentication, authorization, and Swagger OAuth
├── appsettings.json                   # Shared non-sensitive settings
└── appsettings.Development.json       # ⭐ AzureAd configuration shape
```

> This chapter does not contain an `.http` request file.

## 🆕 What's New Compared with Chapter 11

Chapter 11 (`ConfigurationOptionsApi`) is the previous runnable baseline. Chapter 12 contains diagrams and OAuth theory, so it is conceptual context rather than a code baseline.

| Area | Chapter 11 | Chapter 13 |
|---|---|---|
| Authentication | Not configured | JWT bearer authentication through `Microsoft.Identity.Web` |
| Authorization services | Existing middleware call only | `AddAuthorization()` is registered explicitly |
| Controller protection | Product endpoints are anonymous | `[Authorize]` protects the entire controller |
| Swagger | Basic Swagger UI | OAuth 2.0 Authorization Code flow with PKCE |
| Scopes shown by Swagger | None | `Products.Read` and `Products.Write` |
| Pipeline | `UseAuthorization()` | `UseAuthentication()` before `UseAuthorization()` |
| Identity configuration | None | `AzureAd` section in Development settings |
| Existing architecture | Repository, service, middleware, exceptions, Options Pattern | Inherited without a new architectural layer |

## 🏗️ Authentication Request Flow

```text
User
  │
  │ 1. Clicks Authorize in Swagger UI
  ▼
Microsoft Entra ID /authorize endpoint
  │
  │ 2. Authorization code returned to Swagger UI
  ▼
Swagger UI + PKCE
  │
  │ 3. Exchanges code and verifier at /token
  │ 4. Sends Authorization: Bearer <access_token>
  ▼
UseAuthentication()
  │  Validates token and creates the authenticated user
  ▼
UseAuthorization()
  │  Evaluates [Authorize]
  ▼
ProductsController → ProductService → ProductRepository → InMemoryDatabase
```

Authentication answers “who is calling?” Authorization answers “may this caller access the endpoint?” Both are required, and middleware order is significant.

## 💻 Step-by-Step Implementation

### Step 1: Add the identity packages

`AzureOAuthApi.csproj` targets .NET 9 and includes these chapter-specific packages:

```xml
<PackageReference Include="Azure.Extensions.AspNetCore.Configuration.Secrets" Version="1.5.0" />
<PackageReference Include="Azure.Identity" Version="1.21.0" />
<PackageReference Include="Microsoft.Identity.Web" Version="4.7.0" />
```

`Microsoft.Identity.Web` is actively used to configure bearer-token validation. The two Azure packages are installed, but the current `Program.cs` does not connect the application to Azure Key Vault.

The project also contains a `UserSecretsId`, which enables local user-secrets storage. Its machine-specific value is intentionally omitted here.

### Step 2: Define the `AzureAd` configuration shape

The current application reads identity settings from `AzureAd`. Use placeholders in shared documentation and source control:

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

- `TenantId` identifies the Microsoft Entra tenant.
- `ClientId` identifies the application registration used by this lesson.
- `Products.Read` and `Products.Write` must exist under **Expose an API** in Microsoft Entra ID.
- The current code constructs scope URIs as `api://YOUR_CLIENT_ID/Products.Read` and `api://YOUR_CLIENT_ID/Products.Write`.
- Swagger uses Authorization Code flow with PKCE, so do not place a client secret in Swagger UI or committed JSON.

### Step 3: Register authentication and authorization

`Program.cs` sets JWT bearer as the default authentication scheme and binds Microsoft Identity Web to configuration:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddAuthorization();
```

`AddMicrosoftIdentityWebApi` reads the default `AzureAd` section and configures access-token validation for the API.

### Step 4: Configure Swagger OAuth 2.0 scopes

Swagger reads the tenant and client identifiers, then builds Microsoft identity platform v2.0 endpoints:

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

The OpenAPI security requirement requests `Products.Read` by default. Swagger UI then supplies the configured client ID and enables PKCE:

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
    options.OAuthUsePkce();
    options.OAuthScopeSeparator(" ");
});
```

Register the Swagger redirect URI that matches the profile you run, for example:

```text
http://localhost:5213/swagger/oauth2-redirect.html
```

### Step 5: Protect the products controller

`[Authorize]` is applied at controller level, so it covers GET, POST, PUT, DELETE, and bulk-create actions:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(IProductService productService) : ControllerBase
{
    // All actions require an authenticated user.
}
```

Important: the current lesson checks for an authenticated user only. It displays `Products.Read` and `Products.Write` in Swagger, but it does not call `RequiredScope`, define authorization policies, or apply different scope requirements to read and write actions. A valid authenticated token can therefore reach every controller action. Fine-grained scope enforcement is a logical next step.

### Step 6: Order authentication before authorization

The request pipeline preserves inherited exception and custom middleware, then adds the security middleware in this order:

```csharp
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
```

`UseAuthentication()` must run first so it can validate the bearer token and populate `HttpContext.User` before authorization evaluates `[Authorize]`.

## 🔐 Microsoft Entra ID Setup Checklist

Before testing Swagger:

1. Create or select an app registration for the API.
2. Under **Expose an API**, set the Application ID URI to `api://YOUR_CLIENT_ID`.
3. Create delegated scopes named `Products.Read` and `Products.Write`.
4. Configure a platform redirect URI matching Swagger, such as `http://localhost:5213/swagger/oauth2-redirect.html`.
5. Grant the client permission to the required exposed scopes and provide consent when your tenant requires it.
6. Supply the correct tenant and client IDs locally without committing real identifiers or secrets.

The application registration must agree with the exact scope URIs and redirect URI constructed by the code.

## 🔒 Safe Local Configuration

For local development, use user secrets instead of committing real identifiers:

```bash
cd 13-azure-oauth-authorization/AzureOAuthApi
dotnet user-secrets set "AzureAd:TenantId" "YOUR_TENANT_ID"
dotnet user-secrets set "AzureAd:ClientId" "YOUR_CLIENT_ID"
```

Environment variables are another supported configuration source:

```bash
AzureAd__TenantId=YOUR_TENANT_ID
AzureAd__ClientId=YOUR_CLIENT_ID
```

Security notes:

- Never commit real tenant IDs, client IDs, client secrets, tokens, or vault names in tutorial documentation.
- PKCE protects the authorization-code exchange; it does not make a browser capable of safely holding a client secret.
- Avoid logging bearer tokens or sensitive authorization headers.
- Although Azure Key Vault dependencies are present, Key Vault is not active until configuration code is added deliberately.

## 🧪 Test the Protected API with Swagger

### 1. Start the Development profile

Swagger is enabled only in Development. The repository's `http` profile uses Development and listens on port `5213`:

```bash
cd 13-azure-oauth-authorization/AzureOAuthApi
dotnet run --launch-profile http
```

Open:

```text
http://localhost:5213/swagger
```

The `https` launch profile currently sets `ASPNETCORE_ENVIRONMENT` to `Production`, so Swagger is not shown when that profile is used.

### 2. Confirm anonymous access is rejected

Call `GET /api/Products` before authorizing.

Expected result:

```http
HTTP/1.1 401 Unauthorized
```

### 3. Authorize with Microsoft Entra ID

1. Click **Authorize** in Swagger UI.
2. Select the available scope or scopes.
3. Sign in with an account allowed by the tenant.
4. Complete consent if prompted.
5. Call `GET /api/Products` again.

With a valid access token for this API, the endpoint returns `200 OK` and the in-memory product data. An expired token, malformed token, wrong tenant, wrong audience, mismatched scope URI, or redirect-URI mismatch prevents a successful test.

## ▶️ Build and Run

```bash
cd 13-azure-oauth-authorization/AzureOAuthApi
dotnet restore
dotnet build
dotnet run --launch-profile http
```

The project targets .NET 9. Install the .NET 9 SDK before running these commands.

## 🛠️ Troubleshooting

| Symptom | Check |
|---|---|
| Swagger is missing | Run the `http` profile or otherwise set the environment to Development |
| `401 Unauthorized` after sign-in | Verify tenant, audience/client ID, token expiry, and that Swagger sends the bearer token |
| `AADSTS50011` redirect error | Register the exact Swagger OAuth redirect URI |
| Scope does not appear | Expose `Products.Read` and `Products.Write` with the exact Application ID URI |
| Authorization succeeds but write actions are still available | This chapter uses `[Authorize]` only; per-action scope enforcement is not implemented yet |
| Key Vault settings have no effect | The packages are installed, but the current application does not add Key Vault as a configuration provider |

## ✅ Key Takeaways

- `Microsoft.Identity.Web` integrates Microsoft Entra ID token validation with ASP.NET Core JWT bearer authentication.
- `[Authorize]` makes every `ProductsController` action require an authenticated user.
- Swagger can demonstrate Authorization Code flow safely with PKCE.
- Swagger-advertised scopes do not enforce permissions by themselves.
- `UseAuthentication()` must precede `UseAuthorization()`.
- Chapter 13 retains chapter 11's application architecture and adds a security boundary around it.
- Sensitive identifiers belong in user secrets, environment variables, managed cloud configuration, or an intentionally configured secret store—not in tracked tutorial files.

## 🚀 Next Steps

- Enforce `Products.Read` on GET actions and `Products.Write` on mutation actions.
- Add role- or policy-based authorization where the domain requires it.
- Add automated integration tests for anonymous, invalid-token, and authorized requests.
- Configure Azure Key Vault only when the lesson intentionally introduces it and an appropriate credential strategy is available.

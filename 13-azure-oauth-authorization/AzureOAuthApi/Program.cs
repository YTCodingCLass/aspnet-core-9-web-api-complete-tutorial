using Azure.Identity;
using AzureOAuthApi;
using AzureOAuthApi.Configuration;
using AzureOAuthApi.Mappings;
using AzureOAuthApi.Repositories;
using AzureOAuthApi.Services;
using AzureOAuthApi.Handlers;
using AzureOAuthApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);



// ========================================
// Configure Options Pattern
// ========================================
// Bind configuration sections to strongly-typed options classes
// This demonstrates the Options Pattern for configurable middleware
builder.Services.Configure<RequestResponseLoggingOptions>(
    builder.Configuration.GetSection(RequestResponseLoggingOptions.SectionName));

// Add services to the container.
builder.Services.AddControllers();

// Register Problem Details service (required for exception handlers)
builder.Services.AddProblemDetails();
//
// Register exception handlers in order of specificity
// More specific handlers should be registered first
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ========================================
// 🔐 Configure Azure AD Authentication
// ========================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Azure OAuth API", Version = "v1" });

    // 🔐 Add OAuth2 security definition (Authorization Code + PKCE)
    var tenantId = builder.Configuration["AzureAd:TenantId"];
    var clientId = builder.Configuration["AzureAd:ClientId"];

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { $"api://{clientId}/Products.Read", "Read products" },
                    { $"api://{clientId}/Products.Write", "Write products" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            [$"api://{clientId}/Products.Read"]
        }
    });
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register services for dependency injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

// Register middleware services (required for IMiddleware interface)
builder.Services.AddScoped<RequestResponseLoggingMiddleware>();
builder.Services.AddScoped<ResponseTimingMiddleware>();
builder.Services.AddScoped<RequestLoggingMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// *** MIDDLEWARE ORDER MATTERS! ***

// 1. Exception Handler - wraps the entire pipeline to catch all exceptions
// This should be one of the first middleware in the pipeline
app.UseExceptionHandler();

// 2. Custom Middleware - Request/Response Logging (optional - can be verbose)
// Uncomment to enable detailed request/response body logging
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// 3. Custom Middleware - Response Timing
// Measures how long each request takes and adds X-Response-Time header
app.UseMiddleware<ResponseTimingMiddleware>();

// 4. Custom Middleware - Request Logging
// Logs basic request information (method, path, IP)
app.UseMiddleware<RequestLoggingMiddleware>();

// 5. Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        options.OAuthUsePkce(); // ← This enables PKCE in Swagger UI!
        options.OAuthScopeSeparator(" ");
    });
}

// 6. HTTPS Redirection
app.UseHttpsRedirection();

// ⚠️ ORDER MATTERS — Authentication before Authorization!
app.UseAuthentication();   // ← ADD THIS
app.UseAuthorization();    // ← already exists

// 8. Endpoint Routing - maps controllers
app.MapControllers();

app.Run();

//
//api://48c34a32-dbc6-4a25-b36f-4de6c83f9a89
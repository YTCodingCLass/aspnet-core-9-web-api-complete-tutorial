using ConfigurationOptionsApi;
using ConfigurationOptionsApi.Configuration;
using ConfigurationOptionsApi.Mappings;
using ConfigurationOptionsApi.Repositories;
using ConfigurationOptionsApi.Services;
using ConfigurationOptionsApi.Handlers;
using ConfigurationOptionsApi.Middleware;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Custom Middleware API", Version = "v1" });

    // Include XML comments if file exists
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
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
    app.UseSwaggerUI();
}

// 6. HTTPS Redirection
app.UseHttpsRedirection();

// 7. Authorization
app.UseAuthorization();

// 8. Endpoint Routing - maps controllers
app.MapControllers();

app.Run();

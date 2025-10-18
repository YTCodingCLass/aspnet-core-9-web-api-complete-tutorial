using ExceptionHandlingApi;
using ExceptionHandlingApi.Mappings;
using ExceptionHandlingApi.Repositories;
using ExceptionHandlingApi.Services;
using ExceptionHandlingApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Problem Details service (required for exception handlers)
builder.Services.AddProblemDetails();

// Register exception handlers in order of specificity
// More specific handlers should be registered first
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Exception Handling API", Version = "v1" });
    
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

// Service lifetime examples from previous tutorial
builder.Services.AddSingleton<ISingletonService, SingletonService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddTransient<ITransientService, TransientService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

// *** IMPORTANT: Add Exception Handler ***
// This should be one of the first middleware in the pipeline
// The exception handlers will be called in the order they were registered
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

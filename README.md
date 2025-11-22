# 🚀 ASP.NET Core 9 Web API Complete Tutorial Series

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=for-the-badge&logo=dotnet)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![YouTube](https://img.shields.io/badge/YouTube-Tutorial-FF0000?style=for-the-badge&logo=youtube)

> **Complete hands-on tutorial series for building modern Web APIs with ASP.NET Core 9**  
> From absolute beginner to production-ready applications! 🎯

## 📺 YouTube Tutorial Series

This repository contains all the source code for our comprehensive ASP.NET Core 9 Web API tutorial series. Each folder represents a complete chapter with working code examples.

**🔗 [Watch the Full Playlist on YouTube](https://www.youtube.com/playlist?list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA)**

## 📚 Tutorial Structure

| Chapter | Topic | Key Concepts | Duration |
|---------|-------|--------------|----------|
| **01** | [Introduction & Setup](./01-introduction-and-setup/) | Project creation, .NET 9 setup | ⏱️ ~8 min |
| **02-03** | [First Controller & Swagger](./02-03-first-controller-and-swagger/) | Controllers, API documentation | ⏱️ ~15 min |
| **03** | [Swagger UI](./03-http-methods-api/) | API documentation, Testing endpoints | ⏱️ ~16 min |
| **04** | [HTTP Methods API](./04-http-methods-api/) | PUT, PATCH, DELETE, Status codes | ⏱️ ~20 min |
| **05** | [DTOs & Validation](./05-dto-and-validations/) | Data Transfer Objects, ModelState | ⏱️ ~22 min |
| **06** | [AutoMapper Integration](./06-automapper/) | Object mapping, clean architecture | ⏱️ ~23 min |
| **07** | [Dependency Injection](./07-dependency-injection/) | Service lifetimes: Singleton, Scoped, Transient | ⏱️ ~16 min |
| **08** | [Repository & Service Pattern](./08-repository-and-services/) | Repository pattern, Service layer, Clean architecture | ⏱️ ~8 min |
| **09** | [Global Exception Handling](./09-exception-handling/) | IExceptionHandler, RFC 7807, Custom exceptions | ⏱️ ~22 min |
| **10** | [Custom Middleware](./10-custom-middleware/) | IMiddleware interface, Request pipeline, Performance monitoring | ⏱️ ~TBD |

## 🎯 What You'll Learn

### **Core ASP.NET Core Concepts**
- ✅ Setting up ASP.NET Core 9 projects from scratch
- ✅ Building RESTful Web APIs with proper HTTP status codes
- ✅ Implementing all CRUD operations (Create, Read, Update, Delete)
- ✅ Understanding MVC pattern in Web API context

### **API Documentation & Testing**
- ✅ Swagger/OpenAPI integration for automatic documentation
- ✅ XML comments for rich API documentation
- ✅ HTTP request files for testing endpoints
- ✅ ProducesResponseType attributes for clear contracts

### **Data Management & Validation**
- ✅ Data Transfer Objects (DTOs) for clean API design
- ✅ Input validation using DataAnnotations
- ✅ Error handling and custom validation messages
- ✅ AutoMapper for efficient object mapping

### **Dependency Injection & Architecture**
- ✅ Dependency injection and service lifetimes
- ✅ Understanding Singleton, Scoped, and Transient services
- ✅ Service registration and container configuration
- ✅ Best practices for service design and implementation
- ✅ Repository and Service layer patterns
- ✅ Clean architecture with separation of concerns

### **Exception Handling & Error Management**
- ✅ Global exception handling with `IExceptionHandler`
- ✅ RFC 7807 Problem Details standard for error responses
- ✅ Custom exception hierarchy with HTTP status codes
- ✅ Service layer validation and business rule enforcement
- ✅ Environment-aware error details (Development vs Production)
- ✅ Centralized error handling without try-catch blocks

### **Custom Middleware & Request Pipeline**
- ✅ Creating custom middleware using `IMiddleware` interface
- ✅ Understanding the ASP.NET Core middleware pipeline
- ✅ Middleware registration and execution order
- ✅ Request/Response logging and body reading
- ✅ Performance monitoring with response timing
- ✅ Adding custom headers to HTTP responses
- ✅ Stream management for reading request/response bodies

### **Best Practices**
- ✅ Separation of concerns with proper project structure
- ✅ HTTP method semantics (idempotency, safety)
- ✅ Response patterns and status codes
- ✅ Logging and performance considerations

## 🚀 Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Running Any Project
1. **Clone the repository**
   ```bash
   git clone https://github.com/YTCodingCLass/aspnet-core-9-web-api-complete-tutorial.git
   cd aspnet-core-9-web-api-complete-tutorial
   ```

2. **Navigate to desired chapter**
   ```bash
   cd 04-http-methods-api/HttpMethodsApi
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open Swagger UI**
   - Navigate to: `https://localhost:7xxx/swagger`
   - Test all endpoints interactively!

## 📖 Chapter Details

### **Chapter 01: Introduction & Setup**
- Creating your first ASP.NET Core 9 Web API project
- Understanding project structure and files
- Program.cs and minimal hosting model

### **Chapter 02-03: First Controller & Swagger**
- Building your first API controller
- Adding Swagger for API documentation
- XML comments for rich documentation
- Testing with Swagger UI

### **Chapter 03: HTTP Methods API - Part 1**
- Implementing GET and POST methods
- Understanding RESTful principles
- Basic CRUD operations
- Introduction to status codes

### **Chapter 04: HTTP Methods API - Part 2**
- Implementing PUT, PATCH, and DELETE methods
- Understanding method idempotency
- Proper status code usage (200, 201, 204, 400, 404)
- Error handling and logging patterns

### **Chapter 05: DTOs & Validation**
- Data Transfer Objects for clean API design
- Input validation with DataAnnotations
- Custom validation messages
- Separating internal models from API contracts

### **Chapter 06: AutoMapper Integration**
- Installing and configuring AutoMapper in ASP.NET Core
- Creating mapping profiles for DTOs and entities
- Eliminating manual mapping boilerplate code
- Advanced mapping scenarios with stock status calculation
- Best practices for object mapping in Web APIs

### **Chapter 07: Dependency Injection**
- Understanding Dependency Injection principles in ASP.NET Core
- Creating and registering services in the DI container
- Service lifetimes: **Transient**, **Scoped**, and **Singleton**
- Practical differences between service lifetimes with real examples
- When to use each service lifetime in production applications
- Visual demonstration of instance creation and disposal

### **Chapter 08: Repository & Service Pattern**
- Implementing the Repository pattern for data access abstraction
- Building a Service layer for business logic and validation
- Clean architecture with proper separation of concerns
- Dependency injection for loose coupling
- DTO mapping with AutoMapper integration
- Testing strategies with mocked dependencies

### **Chapter 09: Global Exception Handling**
- Implementing global exception handling with `IExceptionHandler` (.NET 8+)
- Creating custom exception hierarchy with HTTP status codes
- RFC 7807 Problem Details standard for consistent error responses
- Exception handler chain (ValidationException → BusinessException → Global)
- Moving validation from controllers to service layer
- Environment-aware error details for security
- Centralized error handling eliminates try-catch blocks in controllers

### **Chapter 10: Custom Middleware**
- Creating custom middleware using the `IMiddleware` interface
- Understanding the ASP.NET Core middleware request pipeline
- Critical importance of middleware registration order
- Request and response logging with complete body capture
- Performance monitoring with `Stopwatch` and response timing headers
- Stream buffering and management for reading request/response bodies
- Adding custom headers like `X-Response-Time` to responses
- Production-ready middleware patterns with dependency injection

## 🔧 Project Structure

```
aspnet-core-9-web-api-tutorial/
├── 01-introduction-and-setup/
│   └── AspNetCore9WebApiIntro/
├── 02-03-first-controller-and-swagger/
│   └── FirstControllerAndSwagger/
├── 03-http-methods-api/
│   └── HttpMethodsApi/
├── 04-http-methods-api/
│   └── HttpMethodsApi/
├── 05-dto-and-validations/
│   └── DtoAndValidation/
├── 06-automapper/
│   └── AutoMapperApi/
├── 07-dependency-injection/
│   └── DependencyInjectionApi/
├── 08-repository-and-services/
│   └── RepositoryAndServicesApi/
├── 09-exception-handling/
│   └── ExceptionHandlingApi/
├── 10-custom-middleware/
│   └── CustomMiddlewareApi/
├── README.md
└── README_AR.md
```

## 🛠️ Technologies Used

- **Framework**: ASP.NET Core 9.0
- **Language**: C# 12
- **Documentation**: Swagger/OpenAPI 3.0
- **Validation**: DataAnnotations
- **Mapping**: AutoMapper
- **IDE**: JetBrains Rider / Visual Studio

## 📋 API Features Covered

### **Product Management API**
- **GET** `/api/products` - Retrieve all products
- **GET** `/api/products/{id}` - Get product by ID
- **POST** `/api/products` - Create new product
- **PUT** `/api/products/{id}` - Update entire product
- **PATCH** `/api/products/{id}` - Partial product update
- **DELETE** `/api/products/{id}` - Delete product

### **Response Formats**
- JSON responses with proper content types
- Consistent error handling
- Validation error details
- Custom headers for created resources

## 🎓 Learning Path

**Recommended order for beginners:**
1. Start with **Chapter 01** for project setup
2. Follow **Chapters 02-03** to understand controllers and Swagger
3. Learn **Chapter 03** for basic HTTP methods (GET, POST)
4. Master **Chapter 04** for advanced HTTP methods (PUT, PATCH, DELETE)
5. Understand **Chapter 05** for professional DTOs and validation
6. Practice **Chapter 06** for AutoMapper integration
7. Study **Chapter 07** for Dependency Injection concepts
8. Build with **Chapter 08** for Repository & Service patterns
9. Complete with **Chapter 09** for Global Exception Handling
10. Advance with **Chapter 10** for Custom Middleware and Request Pipeline

**For experienced developers:**
- Jump to any chapter based on your needs
- Each project is self-contained and runnable

## 🤝 Contributing

Found an issue or want to improve something? Feel free to:
- 🐛 Report bugs
- 💡 Suggest improvements
- 📝 Fix documentation
- ⭐ Star the repository if it helped you!

## 📞 Support & Community

- **YouTube Comments**: Ask questions under each video
- **GitHub Issues**: Report technical problems
- **Discussions**: Share your projects and improvements

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ➡️ What's Next?

🎉 **Congratulations!** You've completed the ASP.NET Core 9 Web API tutorial series!

**You've mastered:**
- ✅ Building RESTful APIs with proper HTTP methods
- ✅ DTOs and validation for clean API contracts
- ✅ AutoMapper for object mapping
- ✅ Dependency Injection and service lifetimes
- ✅ Repository and Service layer patterns
- ✅ Global exception handling with RFC 7807
- ✅ Custom middleware and request pipeline management

**Next learning paths:**
- **Entity Framework Core** for real database integration
- **Authentication & Authorization** with JWT/OAuth
- **Unit Testing** with xUnit and Moq
- **Integration Testing** with WebApplicationFactory
- **Background Services** and hosted services
- **API Versioning** for evolving APIs
- **Caching** with IMemoryCache and Redis
- **Logging** with Serilog and structured logging

## 💡 Dependency Injection Pro Tips

**Understanding Service Lifetimes is crucial for:**
- **Performance optimization** - Choose the right lifetime for your use case
- **Memory management** - Avoid memory leaks and captive dependencies
- **State management** - Understand when instances are created and disposed
- **Testing strategy** - Design services that are easy to mock and test

**Key concepts demonstrated in Chapter 07:**
- ✅ **Singleton**: One instance for entire application lifetime
- ✅ **Scoped**: One instance per HTTP request (perfect for DbContext)
- ✅ **Transient**: New instance every time it's injected
- ✅ **Visual demonstration** with console logging and GUID tracking
- ✅ **Performance implications** of each service lifetime
- ✅ **Common pitfalls** and how to avoid them

## 💡 Exception Handling Pro Tips

**Global Exception Handling provides:**
- **Centralized error management** - All exceptions handled in one place
- **RFC 7807 compliance** - Standard problem details format for consistency
- **Clean controllers** - No try-catch blocks cluttering your code
- **Environment-aware** - Detailed errors in dev, sanitized in production
- **Type-safe exceptions** - Custom exception hierarchy with HTTP status codes
- **Service layer validation** - Reusable validation across all entry points

**Key benefits demonstrated in Chapter 09:**
- ✅ **Zero try-catch** blocks in controllers
- ✅ **Consistent error** responses across the entire API
- ✅ **Structured exceptions** with proper HTTP status codes
- ✅ **Validation in services** for better reusability and testing
- ✅ **Production-ready** security with environment-aware error details

## 💡 Custom Middleware Pro Tips

**Custom Middleware enables:**
- **Cross-cutting concerns** - Handle logging, timing, and headers in one place
- **IMiddleware interface** - Dependency injection support with scoped services
- **Pipeline order control** - Precise control over request/response flow
- **Performance monitoring** - Automatic response time tracking and slow request detection
- **Request/Response logging** - Complete body capture for debugging
- **Reusable components** - Write once, use across all endpoints

**Key concepts demonstrated in Chapter 10:**
- ✅ **IMiddleware interface** - Modern approach with DI support
- ✅ **Primary constructors** - C# 12 feature for cleaner code
- ✅ **Middleware order** - Exception handlers first, routing last
- ✅ **Stream buffering** - EnableBuffering() for reading request body multiple times
- ✅ **OnStarting callback** - Add headers before response is sent
- ✅ **Performance monitoring** - Stopwatch and X-Response-Time header

## 💡 AutoMapper Pro Tips

**AutoMapper eliminates repetitive mapping code and provides:**
- **Automatic mapping** between DTOs and entities with minimal configuration
- **Stock status calculation** with custom mapping logic for business rules
- **Cleaner controllers** with reduced boilerplate - from 20+ lines to 1-2 lines per mapping
- **Centralized mapping configuration** in profiles for better maintainability
- **Performance benefits** through compiled mappings and cached expressions

**Key benefits demonstrated in Chapter 06:**
- ✅ **90% reduction** in mapping code lines
- ✅ **Centralized configuration** in MappingProfile.cs
- ✅ **Advanced scenarios** like stock status calculation
- ✅ **Better separation of concerns** between controllers and mapping logic

## 🙏 Acknowledgments

- Microsoft for the amazing .NET platform
- Swashbuckle team for Swagger integration
- AutoMapper contributors
- The awesome .NET community

---

**⭐ Don't forget to star this repository if it helped you learn ASP.NET Core!**

**🔔 Subscribe to our YouTube channel for more .NET tutorials!**
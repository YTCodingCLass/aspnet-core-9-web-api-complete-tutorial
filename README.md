# 🚀 ASP.NET Core 9 Web API Complete Tutorial Series

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=for-the-badge&logo=dotnet)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![YouTube](https://img.shields.io/badge/YouTube-Tutorial-FF0000?style=for-the-badge&logo=youtube)

> **Complete hands-on tutorial series for building modern Web APIs with ASP.NET Core 9**  
> From absolute beginner to production-ready applications! 🎯

## 📺 YouTube Tutorial Series

This repository contains all the source code for our comprehensive ASP.NET Core 9 Web API tutorial series. Each folder represents a complete chapter with working code examples.

**🔗 [Watch the Full Playlist on YouTube](#)** *(Add your playlist link here)*

## 📚 Tutorial Structure

| Chapter | Topic | Key Concepts | Duration |
|---------|-------|--------------|----------|
| **01** | [Introduction & Setup](./01-introduction-and-setup/) | Project creation, .NET 9 setup | ⏱️ ~15 min |
| **02-03** | [First Controller & Swagger](./02-03-first-controller-and-swagger/) | Controllers, API documentation | ⏱️ ~25 min |
| **04** | [HTTP Methods API](./04-http-methods-api/) | GET, POST, PUT, PATCH, DELETE | ⏱️ ~35 min |
| **05** | [DTOs & Validation](./05-dto-and-validations/) | Data Transfer Objects, ModelState | ⏱️ ~30 min |
| **06** | [AutoMapper Integration](./06-automapper/) | Object mapping, clean architecture | ⏱️ ~20 min |

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

### **Best Practices**
- ✅ Dependency injection and logging
- ✅ Separation of concerns with proper project structure
- ✅ HTTP method semantics (idempotency, safety)
- ✅ Response patterns and status codes

## 🚀 Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Running Any Project
1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/aspnet-core-9-web-api-complete-tutorial.git
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

### **Chapter 04: HTTP Methods API**
- Implementing all HTTP methods (GET, POST, PUT, PATCH, DELETE)
- Understanding RESTful principles
- Proper status code usage
- Error handling and logging

### **Chapter 05: DTOs & Validation**
- Data Transfer Objects for clean API design
- Input validation with DataAnnotations
- Custom validation messages
- Separating internal models from API contracts

### **Chapter 06: AutoMapper Integration**
- Installing and configuring AutoMapper
- Mapping between entities and DTOs
- Reducing boilerplate mapping code
- Clean architecture patterns

## 🔧 Project Structure

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
│   └── AutoMapper/
└── README.md
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
2. Follow **Chapters 02-03** to understand controllers
3. Master **Chapter 04** for HTTP methods
4. Learn **Chapter 05** for professional DTOs
5. Complete with **Chapter 06** for AutoMapper

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

## 🙏 Acknowledgments

- Microsoft for the amazing .NET platform
- Swashbuckle team for Swagger integration
- AutoMapper contributors
- The awesome .NET community

---

**⭐ Don't forget to star this repository if it helped you learn ASP.NET Core!**

**🔔 Subscribe to our YouTube channel for more .NET tutorials!**
# Chapter 01: Introduction & ASP.NET Core 9 Setup

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=flat-square&logo=dotnet)

## 📺 YouTube Video
**🔗 [Watch Chapter 01: Introduction & Setup](#)** *(Add your video link here)*

## 🎯 Learning Objectives

By the end of this chapter, you'll understand:
- ✅ What is ASP.NET Core and why use version 9
- ✅ How to create a new Web API project from scratch
- ✅ Understanding the default project structure
- ✅ How the new minimal hosting model works in .NET 9

## 🚀 What We Build

A basic ASP.NET Core 9 Web API project with:
- Minimal hosting configuration
- Default development environment setup
- Basic project structure understanding

## 📁 Project Structure

```
AspNetCore9WebApiIntro/
├── Program.cs                  # Application entry point
├── appsettings.json           # Application configuration
├── appsettings.Development.json # Development settings
├── Properties/
│   └── launchSettings.json    # Debug/launch profiles
└── AspNetCore9WebApiIntro.csproj # Project file
```

## 🔧 Key Files Explained

### **Program.cs**
The heart of your application using minimal hosting model:
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Run();
```

### **appsettings.json**
Configuration for your application:
- Database connections
- API keys
- Application-specific settings

### **launchSettings.json**
Development environment configuration:
- Development server ports
- Environment variables
- Browser launch settings

## 💻 Running the Project

1. **Navigate to the project folder**
   ```bash
   cd 01-introduction-and-setup/AspNetCore9WebApiIntro
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Test the endpoint**
   - Open browser: `https://localhost:7xxx`
   - You should see: "Hello World!"

## 🔍 What's New in .NET 9

- **Improved performance** with better garbage collection
- **Enhanced minimal APIs** with more features
- **Better developer experience** with improved tooling
- **Native AOT improvements** for faster startup times

## 📝 Prerequisites

- Basic understanding of C# programming
- Familiarity with HTTP concepts
- .NET 9 SDK installed
- Your favorite IDE (Visual Studio, VS Code, or Rider)

## ➡️ Next Steps

Ready to add some real functionality? Continue to:
**[Chapter 02-03: First Controller & Swagger](../02-03-first-controller-and-swagger/)**

## 🤔 Common Issues

**Port already in use?**
- Check `Properties/launchSettings.json` and change the port numbers

**Build errors?**
- Ensure you have .NET 9 SDK installed: `dotnet --version`
- Run `dotnet clean` then `dotnet restore`

---

**💡 Tip**: This is just the foundation! The real magic happens in the next chapters where we build a complete Product API.
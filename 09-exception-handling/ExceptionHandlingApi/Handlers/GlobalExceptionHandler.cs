using System.Diagnostics;
using System.Text.Json;
using ExceptionHandlingApi.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionHandlingApi.Handlers;

/// <summary>
/// Global exception handler using the built-in IExceptionHandler interface (NET 8+)
/// This approach is more integrated with ASP.NET Core's infrastructure
/// </summary>
public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the exception
        logger.LogError(exception,
            "Exception occurred: {Message}", exception.Message);

        // Create problem details based on exception type
        var problemDetails = CreateProblemDetails(httpContext, exception);

        // Set the response status code
        httpContext.Response.StatusCode = problemDetails.Status ??
            StatusCodes.Status500InternalServerError;

        // Set content type
        httpContext.Response.ContentType = "application/problem+json";

        // Write the problem details response directly as JSON
        // This bypasses IProblemDetailsService which was sanitizing our custom details
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, options, cancellationToken);
        return true;
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = exception switch
        {
            ValidationException validationEx => CreateValidationProblemDetails(context, validationEx),
            BaseException baseEx => CreateBaseProblemDetails(context, baseEx),
            _ => CreateGenericProblemDetails(context, exception)
        };

        // Add common extensions
        AddCommonExtensions(problemDetails, context, exception);

        return problemDetails;
    }

    private ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext context,
        ValidationException validationException)
    {
        var problemDetails = new ValidationProblemDetails(validationException.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = validationException.StatusCode,
            Detail = validationException.Message,
            Instance = context.Request.Path,
            Extensions =
            {
                ["errorCode"] = validationException.ErrorCode
            }
        };

        return problemDetails;
    }

    private ProblemDetails CreateBaseProblemDetails(
        HttpContext context,
        BaseException baseException)
    {
        return new ProblemDetails
        {
            Type = GetProblemType(baseException.StatusCode),
            Title = GetTitle(baseException.StatusCode),
            Status = baseException.StatusCode,
            Detail = baseException.Message,
            Instance = context.Request.Path,
            Extensions =
            {
                ["errorCode"] = baseException.ErrorCode
            }
        };
    }

    private ProblemDetails CreateGenericProblemDetails(
        HttpContext context,
        Exception exception)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var detail = environment.IsDevelopment()
            ? exception.Message
            : "An error occurred while processing your request.";

        return new ProblemDetails
        {
            Type = GetProblemType(statusCode),
            Title = GetTitle(statusCode),
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path,
            Extensions =
            {
                ["errorCode"] = "INTERNAL_SERVER_ERROR"
            }
        };
    }

    private void AddCommonExtensions(
        ProblemDetails problemDetails,
        HttpContext context,
        Exception exception)
    {
        // Add timestamp
        problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;

        // Add trace identifier for correlation
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        problemDetails.Extensions["traceId"] = traceId;

        // Add request ID if available
        if (context.Request.Headers.TryGetValue("X-Request-Id", out var requestId))
        {
            problemDetails.Extensions["requestId"] = requestId.ToString();
        }

        // In development, add additional debugging information
        if (environment.IsDevelopment())
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    message = exception.InnerException.Message,
                    type = exception.InnerException.GetType().Name
                };
            }
        }
    }

    private static string GetProblemType(int statusCode)
    {
        // You can customize these to point to your own error documentation
        return statusCode switch
        {
            400 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            401 => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            403 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
            404 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            409 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
            422 => "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2",
            500 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6"
        };
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            422 => "Unprocessable Entity",
            500 => "Internal Server Error",
            503 => "Service Unavailable",
            _ => "An error occurred"
        };
    }
}

using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CustomMiddlewareApi.Handlers;

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
        var problemDetails = CreateGenericProblemDetails(context, exception);

        // Add common extensions
        AddCommonExtensions(problemDetails, context, exception);

        return problemDetails;
    }

    private ProblemDetails CreateGenericProblemDetails(
        HttpContext context,
        Exception exception)
    {
        var detail = environment.IsDevelopment()
            ? exception.Message
            : "An error occurred while processing your request.";

        return new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
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
}

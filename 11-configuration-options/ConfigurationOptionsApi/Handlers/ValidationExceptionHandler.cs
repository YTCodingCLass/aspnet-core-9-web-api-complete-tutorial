using System.Diagnostics;
using System.Text.Json;
using ConfigurationOptionsApi.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationOptionsApi.Handlers;

/// <summary>
/// Specialized handler for validation exceptions
/// You can create multiple specific handlers and chain them
/// </summary>
public class ValidationExceptionHandler(
    ILogger<ValidationExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Only handle ValidationException
        if (exception is not ValidationException validationException)
        {
            // Return false to let the next handler in the chain handle it
            return false;
        }

        logger.LogWarning(
            "Validation error occurred: {ErrorCount} validation errors",
            validationException.Errors.Count);

        var problemDetails = new ValidationProblemDetails(validationException.Errors)
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = validationException.Message,
            Instance = httpContext.Request.Path
        };

        // Add custom extensions
        problemDetails.Extensions["errorCode"] = "VALIDATION_ERROR";
        problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        httpContext.Response.ContentType = "application/problem+json";

        // Write response directly as JSON to preserve custom extensions
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, options, cancellationToken);
        return true;
    }
}

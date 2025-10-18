using System.Diagnostics;
using System.Text.Json;
using ExceptionHandlingApi.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionHandlingApi.Handlers;

/// <summary>
/// Handler for business/domain exceptions that inherit from BaseException
/// </summary>
public class BusinessExceptionHandler(
    ILogger<BusinessExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Skip if not a BaseException or if it's a ValidationException (handled by another handler)
        if (exception is not BaseException baseException || exception is ValidationException)
        {
            return false;
        }

        logger.LogWarning(
            exception,
            "Business exception occurred: {ErrorCode} - {Message}",
            baseException.ErrorCode,
            baseException.Message);

        var problemDetails = new ProblemDetails
        {
            Type = GetProblemTypeUrl(baseException.StatusCode),
            Title = GetStatusTitle(baseException.StatusCode),
            Status = baseException.StatusCode,
            Detail = baseException.Message,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                // Add extensions
                ["errorCode"] = baseException.ErrorCode,
                ["timestamp"] = DateTimeOffset.UtcNow,
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };

        httpContext.Response.StatusCode = baseException.StatusCode;
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

    private static string GetProblemTypeUrl(int statusCode)
    {
        return statusCode switch
        {
            400 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            401 => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            403 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
            404 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            409 => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
            _ => $"https://httpstatuses.com/{statusCode}"
        };
    }

    private static string GetStatusTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            _ => "An error occurred"
        };
    }
}

using System.Text;
using ConfigurationOptionsApi.Configuration;
using Microsoft.Extensions.Options;

namespace ConfigurationOptionsApi.Middleware;

/// <summary>
/// BEFORE: Always logs full request/response bodies - can be overwhelming in production
/// AFTER: Uses Options Pattern - can toggle body/header logging and set size limits
/// </summary>
public class RequestResponseLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RequestResponseLoggingOptions _options;
 
    public RequestResponseLoggingMiddleware(
        ILogger<RequestResponseLoggingMiddleware> logger,
        IOptions<RequestResponseLoggingOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if logging is enabled via configuration
        if (!_options.IsEnabled)
        {
            await next(context);
            return;
        }

        // Log Request
        await LogRequest(context);

        // Copy the original response stream
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        // Execute the next middleware
        await next(context);

        // Log Response
        await LogResponse(context);

        // Copy the contents of the new response stream to the original stream
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task LogRequest(HttpContext context)
    {
        var logBuilder = new StringBuilder();
        logBuilder.AppendLine("HTTP Request Information:");
        logBuilder.AppendLine($"Method: {context.Request.Method}");
        logBuilder.AppendLine($"Path: {context.Request.Path}");
        logBuilder.AppendLine($"QueryString: {context.Request.QueryString}");

        // Include headers if configured
        if (_options.IncludeRequestHeaders)
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in context.Request.Headers)
            {
                logBuilder.AppendLine($"  {header.Key}: {header.Value}");
            }
        }

        // Include body if configured
        if (_options.IncludeRequestBody)
        {
            context.Request.EnableBuffering();

            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            // Truncate if body exceeds max size
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [TRUNCATED]";
            }

            logBuilder.AppendLine($"Body: {body}");
        }

        _logger.LogInformation(logBuilder.ToString());
    }

    private async Task LogResponse(HttpContext context)
    {
        var logBuilder = new StringBuilder();
        logBuilder.AppendLine("HTTP Response Information:");
        logBuilder.AppendLine($"StatusCode: {context.Response.StatusCode}");

        // Include headers if configured
        if (_options.IncludeResponseHeaders)
        {
            logBuilder.AppendLine("Headers:");
            foreach (var header in context.Response.Headers)
            {
                logBuilder.AppendLine($"  {header.Key}: {header.Value}");
            }
        }

        // Include body if configured
        if (_options.IncludeResponseBody)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Truncate if body exceeds max size
            if (body.Length > _options.MaxBodySizeToLog)
            {
                body = body.Substring(0, _options.MaxBodySizeToLog) + "... [TRUNCATED]";
            }

            logBuilder.AppendLine($"Body: {body}");
        }

        _logger.LogInformation(logBuilder.ToString());
    }
}

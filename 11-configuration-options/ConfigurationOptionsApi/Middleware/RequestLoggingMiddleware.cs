using System.Diagnostics;

namespace ConfigurationOptionsApi.Middleware;

public class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Before the request
        logger.LogInformation(
            "Incoming Request: {Method} {Path} from {RemoteIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        // Call the next middleware in the pipeline
        await next(context);

        // After the response
        logger.LogInformation(
            "Outgoing Response: {StatusCode} for {Method} {Path}",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path);
    }
}

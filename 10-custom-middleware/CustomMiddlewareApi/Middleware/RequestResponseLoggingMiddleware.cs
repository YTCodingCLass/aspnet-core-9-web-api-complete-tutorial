using System.Text;

namespace CustomMiddlewareApi.Middleware;

public class RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
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
        context.Request.EnableBuffering();

        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        logger.LogInformation(
            "HTTP Request Information:\n" +
            "Method: {Method}\n" +
            "Path: {Path}\n" +
            "QueryString: {QueryString}\n" +
            "Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            body);
    }

    private async Task LogResponse(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        logger.LogInformation(
            "HTTP Response Information:\n" +
            "StatusCode: {StatusCode}\n" +
            "Body: {Body}",
            context.Response.StatusCode,
            body);
    }
}

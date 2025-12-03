using System.Text;

namespace CustomMiddlewareApi.Middleware;

public class RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger) : IMiddleware
{
    private const bool IsEnabled = true;
    private const bool LogRequestBody = true;
    private const bool LogResponseBody = true;
    private const int MaxBodyLogSize = 4096;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!IsEnabled)
        {
            await next(context);
            return;
        }

        await LogRequest(context);

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next(context);

        await LogResponse(context);

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        var requestInfo = $"HTTP Request Information:\n" +
                          $"Method: {context.Request.Method}\n" +
                          $"Path: {context.Request.Path}\n" +
                          $"QueryString: {context.Request.QueryString}";

        if (LogRequestBody)
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (body.Length > MaxBodyLogSize)
            {
                body = body[..MaxBodyLogSize] + "... [TRUNCATED]";
            }

            requestInfo += $"\nBody: {body}";
        }

        logger.LogInformation(requestInfo);
    }

    private async Task LogResponse(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var responseInfo = $"HTTP Response Information:\n" +
                           $"StatusCode: {context.Response.StatusCode}";

        if (LogResponseBody)
        {
            if (body.Length > MaxBodyLogSize)
            {
                body = body[..MaxBodyLogSize] + "... [TRUNCATED]";
            }

            responseInfo += $"\nBody: {body}";
        }

        logger.LogInformation(responseInfo);
    }
}
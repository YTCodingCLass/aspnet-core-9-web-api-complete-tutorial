using System.Diagnostics;

namespace ConfigurationOptionsApi.Middleware;

public class ResponseTimingMiddleware(ILogger<ResponseTimingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Start the stopwatch
        var stopwatch = Stopwatch.StartNew();

        // Hook into OnStarting to add the header before the response is sent
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            context.Response.Headers.Append("X-Response-Time", $"{elapsedMilliseconds}ms");
            return Task.CompletedTask;
        });

        // Execute the next middleware
        await next(context);

        // Stop the stopwatch (if not already stopped)
        stopwatch.Stop();

        // Log the elapsed time
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        logger.LogInformation(
            "Request {Method} {Path} completed in {ElapsedMilliseconds}ms with status {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            elapsedMilliseconds,
            context.Response.StatusCode);

        // Warn if request took too long
        if (elapsedMilliseconds > 1000)
        {
            logger.LogWarning(
                "SLOW REQUEST: {Method} {Path} took {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds);
        }
    }
}

namespace ConfigurationOptionsApi.Configuration;

/// <summary>
/// Configuration options for the RequestResponseLoggingMiddleware.
/// Demonstrates the Options Pattern for making middleware configurable.
/// </summary>
public class RequestResponseLoggingOptions
{
    /// <summary>
    /// The configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "RequestResponseLogging";

    /// <summary>
    /// Enable or disable detailed request/response logging
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Include request headers in logs
    /// </summary>
    public bool IncludeRequestHeaders { get; set; } = false;

    /// <summary>
    /// Include response headers in logs
    /// </summary>
    public bool IncludeResponseHeaders { get; set; } = false;

    /// <summary>
    /// Include request body in logs
    /// </summary>
    public bool IncludeRequestBody { get; set; } = true;

    /// <summary>
    /// Include response body in logs
    /// </summary>
    public bool IncludeResponseBody { get; set; } = true;

    /// <summary>
    /// Maximum body size to log (in bytes). Bodies larger than this will be truncated.
    /// </summary>
    public int MaxBodySizeToLog { get; set; } = 4096;
}

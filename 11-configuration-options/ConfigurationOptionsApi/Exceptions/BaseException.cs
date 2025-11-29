namespace ConfigurationOptionsApi.Exceptions;

/// <summary>
/// Base exception class for all custom exceptions in the application
/// </summary>
public abstract class BaseException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected BaseException(string message, int statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    protected BaseException(string message, int statusCode, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

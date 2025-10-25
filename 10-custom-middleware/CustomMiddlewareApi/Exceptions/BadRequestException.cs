namespace CustomMiddlewareApi.Exceptions;

/// <summary>
/// Exception thrown when a request is invalid or malformed
/// </summary>
public class BadRequestException : BaseException
{
    public BadRequestException(string message)
        : base(message, StatusCodes.Status400BadRequest, "BAD_REQUEST")
    {
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, StatusCodes.Status400BadRequest, "BAD_REQUEST", innerException)
    {
    }
}

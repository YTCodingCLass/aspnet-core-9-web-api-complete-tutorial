namespace CustomMiddlewareApi.Exceptions;

/// <summary>
/// Exception thrown when a user is authenticated but doesn't have permission to access a resource
/// </summary>
public class ForbiddenException(string message = "Access forbidden.")
    : BaseException(message, StatusCodes.Status403Forbidden, "FORBIDDEN");

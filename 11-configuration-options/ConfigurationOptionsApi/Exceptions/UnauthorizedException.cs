namespace ConfigurationOptionsApi.Exceptions;

/// <summary>
/// Exception thrown when a user is not authenticated or lacks valid credentials
/// </summary>
public class UnauthorizedException(string message = "Unauthorized access.")
    : BaseException(message, StatusCodes.Status401Unauthorized, "UNAUTHORIZED");

namespace CustomMiddlewareApi.Exceptions;

/// <summary>
/// Exception thrown when there is a conflict with the current state of a resource
/// </summary>
public class ConflictException(string message) : BaseException(message, StatusCodes.Status409Conflict, "CONFLICT");

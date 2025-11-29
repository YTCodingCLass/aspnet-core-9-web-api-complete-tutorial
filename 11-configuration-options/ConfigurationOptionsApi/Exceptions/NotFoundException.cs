namespace ConfigurationOptionsApi.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : BaseException
{
    public NotFoundException(string message)
        : base(message, StatusCodes.Status404NotFound, "NOT_FOUND")
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found.", StatusCodes.Status404NotFound, "NOT_FOUND")
    {
    }
}

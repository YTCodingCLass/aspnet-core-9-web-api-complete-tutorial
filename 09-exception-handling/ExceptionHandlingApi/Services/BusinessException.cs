// Custom exception for business logic violations
namespace ExceptionHandlingApi.Services;
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
    public BusinessException(string message, Exception innerException) : base(message, innerException) { }
}

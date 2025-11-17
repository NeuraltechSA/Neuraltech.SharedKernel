
namespace Neuraltech.SharedKernel.Domain.Exceptions;

public class InvalidUrlException : DomainException
{
    public InvalidUrlException(string message) : base(message)
    {
    }

    public InvalidUrlException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
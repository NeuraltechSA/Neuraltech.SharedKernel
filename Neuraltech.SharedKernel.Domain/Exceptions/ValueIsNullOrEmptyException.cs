namespace Neuraltech.SharedKernel.Domain.Exceptions;

public class ValueIsNullOrEmptyException : DomainException
{
    public ValueIsNullOrEmptyException(string message) : base(message)
    {}

    public static ValueIsNullOrEmptyException Create(string? value)
    {
        return new ValueIsNullOrEmptyException($"The value cannot be null or empty.");
    }
}
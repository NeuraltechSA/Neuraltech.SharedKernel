namespace Neuraltech.SharedKernel.Domain.Exceptions;

public class ValueOutOfRangeException : DomainException
{
    public ValueOutOfRangeException(string message) : base(message)
    { }

    public static ValueOutOfRangeException Create<T>(T value, T min, T max)
    {
        return new ValueOutOfRangeException($" The value should be between {min} and {max}. Current value: {value}.");
    }


}
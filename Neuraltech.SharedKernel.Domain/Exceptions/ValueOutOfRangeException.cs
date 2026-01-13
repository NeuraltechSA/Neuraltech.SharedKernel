namespace Neuraltech.SharedKernel.Domain.Exceptions;

public class ValueOutOfRangeException : DomainException
{
    public ValueOutOfRangeException(string message) : base(message)
    { }

    public static ValueOutOfRangeException Create<T>(T value, T min, T max)
    {
        return new ValueOutOfRangeException($" The value should be between {min} and {max}. Current value: {value}.");
    }

    public static ValueOutOfRangeException CreateGreaterOrEqualThan<T>(T value, T minimum)
    {
        return new ValueOutOfRangeException($"The value should be greater than or equal to {minimum}. Current value: {value}.");
    }

    public static ValueOutOfRangeException CreateLessOrEqualThan<T>(T value, T maximum)
    {
        return new ValueOutOfRangeException($"The value should be less than or equal to {maximum}. Current value: {value}.");
    }

    public static ValueOutOfRangeException CreateGreaterThan<T>(T value, T minimum)
    {
        return new ValueOutOfRangeException($"The value should be greater than {minimum}. Current value: {value}.");
    }

    public static ValueOutOfRangeException CreateLessThan<T>(T value, T maximum)
    {
        return new ValueOutOfRangeException($"The value should be less than {maximum}. Current value: {value}.");
    }
}
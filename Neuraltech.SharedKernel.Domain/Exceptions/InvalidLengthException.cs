namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class InvalidLengthException : DomainException
    {
        public InvalidLengthException(string message) : base(message)
        {}

        public static InvalidLengthException Create<T>(IEnumerable<T> value, int length)
        {
            return new InvalidLengthException($"The length should be {length}. Current length: {value.Count()}.");
        }

        public static InvalidLengthException Create<T>(IEnumerable<T> value, int minLength, int maxLength)
        {
            return new InvalidLengthException($"The length must be between {minLength} and {maxLength}. Current length: {value.Count()}.");
        }

        public static InvalidLengthException CreateMinLength<T>(IEnumerable<T> value, int minLength)
        {
            return new InvalidLengthException($"The length should be at least {minLength}. Current length: {value.Count()}.");
        }
        public static InvalidLengthException CreateMaxLength<T>(IEnumerable<T> value, int maxLength)
        {
            return new InvalidLengthException($"The length should be at most {maxLength}. Current length: {value.Count()}.");
        }


    }
}

namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class UnexpectedValueException : DomainException
    {
        public UnexpectedValueException(string message) : base(message)
        {
        }

        public static UnexpectedValueException CreateFromAllowed<T>(T value, IEnumerable<T> allowedValues)
        {
            var allowedValuesString = string.Join(", ", allowedValues);
            return new UnexpectedValueException($"The value '{value}' is not valid. Allowed values are: {allowedValuesString}.");
        }

        public static UnexpectedValueException CreateFromForbidden<T>(T value, IEnumerable<T> forbiddenValues)
        {
            var forbiddenValuesString = string.Join(", ", forbiddenValues);
            return new UnexpectedValueException($"The value '{value}' is not valid. Forbidden values are: {forbiddenValuesString}.");
        }

    }
}

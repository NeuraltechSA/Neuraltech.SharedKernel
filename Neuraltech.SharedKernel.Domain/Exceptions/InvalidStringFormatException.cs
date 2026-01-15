namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class InvalidStringFormatException : DomainException
    {
        public InvalidStringFormatException(string message) : base(message)
        {
        }

        public static InvalidStringFormatException CreateStartsWith(string value, string prefix)
        {
            return new InvalidStringFormatException($"The value '{value}' should start with '{prefix}'.");
        }

        public static InvalidStringFormatException CreateEndsWith(string value, string suffix)
        {
            return new InvalidStringFormatException($"The value '{value}' should end with '{suffix}'.");
        }

        public static InvalidStringFormatException CreateContains(string value, string substring)
        {
            return new InvalidStringFormatException($"The value '{value}' should contain '{substring}'.");
        }

        public static InvalidStringFormatException CreateNotContains(string value, string substring)
        {
            return new InvalidStringFormatException($"The value '{value}' should not contain '{substring}'.");
        }

        public static InvalidStringFormatException CreatePattern(string value, string pattern)
        {
            return new InvalidStringFormatException($"The value '{value}' does not match the required pattern '{pattern}'.");
        }
    }
}

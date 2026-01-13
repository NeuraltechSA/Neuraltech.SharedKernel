namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class InvalidPrefixException : DomainException
    {
        public InvalidPrefixException(string message) : base(message)
        {
        }

        public static InvalidPrefixException Create(string value, string expectedPrefix)
        {
            return new InvalidPrefixException($"The value '{value}' should start with '{expectedPrefix}'.");
        }
    }
}
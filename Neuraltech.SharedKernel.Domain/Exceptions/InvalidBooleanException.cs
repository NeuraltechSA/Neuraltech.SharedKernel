
namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class InvalidBooleanException : DomainException
    {
        public InvalidBooleanException(string message) : base(message) { }

        public static InvalidBooleanException TrueExpected()
            => new InvalidBooleanException($"Expected value to be true.");

        public static InvalidBooleanException FalseExpected()
            => new InvalidBooleanException($"Expected value to be false.");
    }
}
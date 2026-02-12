namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class ExpectedNullException : DomainException
    {
        public ExpectedNullException(string paramName, string? message = null) 
            : base($"Expected {paramName} to be null. {message}")
        {
        }

        public static ExpectedNullException Create(string? message = null)
        {
            return new ExpectedNullException("value", message);
        }
    }
}
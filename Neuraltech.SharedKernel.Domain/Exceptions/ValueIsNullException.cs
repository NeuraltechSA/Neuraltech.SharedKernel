using System;

namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class ValueIsNullException : DomainException
    {
        public ValueIsNullException(string? paramName = null)
            : base(paramName is null ? "Value cannot be null." : $"Value '{paramName}' cannot be null.") { }

        public static ValueIsNullException Create(string? paramName = null) => new(paramName);
    }
}

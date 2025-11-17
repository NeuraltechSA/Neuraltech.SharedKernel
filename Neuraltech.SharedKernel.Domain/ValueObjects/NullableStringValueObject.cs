using System;

namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public abstract record NullableStringValueObject : ValueObject<string?>
{
    public NullableStringValueObject(string? value) : base(value)
    {
        EnsureIsNotEmpty(value);
    }
    private void EnsureIsNotEmpty(string? value)
    {
        if (value != null && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El valor no puede estar vacío");
        }
    }
}
using Neuraltech.SharedKernel.Domain.Services;
using System;

namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public abstract record StringValueObject : ValueObject<string>
{
    public StringValueObject(string value) : base(value)
    {
        Ensure.NotNullOrWhiteSpace(value);
    }
}
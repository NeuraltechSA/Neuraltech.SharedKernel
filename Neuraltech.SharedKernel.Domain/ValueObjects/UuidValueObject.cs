namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public abstract  record UuidValueObject : ValueObject<Guid>
{
    protected UuidValueObject(Guid value) : base(value)
    {
    }
}
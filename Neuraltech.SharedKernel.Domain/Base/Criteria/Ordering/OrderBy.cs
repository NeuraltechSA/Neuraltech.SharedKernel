namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

public sealed record OrderBy
{
    public string Value { get; init; }

    private OrderBy(string value)
    {
        Value = value;
    }

    public static OrderBy Create(string propertyName)
    {
        return new OrderBy(propertyName);
    }

    public override string ToString() => Value;
}

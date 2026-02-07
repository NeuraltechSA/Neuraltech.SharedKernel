namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;

public sealed record FilterField
{
    public string Value { get; init; }

    private FilterField(string value)
    {
        Value = value;
    }

    public static FilterField Create(string fieldName)
    {
        return new FilterField(fieldName);
    }

    public override string ToString() => Value;
}

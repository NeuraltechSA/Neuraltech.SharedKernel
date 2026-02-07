namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;

public sealed record FilterValue(object? Value)
{
    public override string ToString() => Value?.ToString() ?? "null";
}

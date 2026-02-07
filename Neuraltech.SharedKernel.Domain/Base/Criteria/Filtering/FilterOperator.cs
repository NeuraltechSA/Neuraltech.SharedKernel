namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;

public sealed record FilterOperator(FilterOperators Value)
{
    public override string ToString() => Value.ToString();
}

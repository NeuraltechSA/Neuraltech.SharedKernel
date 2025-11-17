namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

public sealed record OrderBy(string Value)
{

    public static OrderBy Create(string value) => new OrderBy(value);
}
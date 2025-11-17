namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

public sealed record OrderType(OrderTypes Value)
{
    public static OrderType Create(OrderTypes value) => new OrderType(value);
}
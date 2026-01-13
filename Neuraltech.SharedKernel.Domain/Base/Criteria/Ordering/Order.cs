namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

public sealed record Order
{
    private readonly OrderBy _orderBy;
    private readonly OrderType _orderType;

    private Order(OrderBy orderBy, OrderType orderType)
    {
        _orderBy = orderBy;
        _orderType = orderType;
    }

    public static Order Create(string propertyName, OrderTypes orderType)
    {
        var orderBy = OrderBy.Create(propertyName);
        return new Order(orderBy, OrderType.Create(orderType));
    }

    public string GetOrderBy() => _orderBy.Value;
    public OrderTypes GetOrderType() => _orderType.Value;
}
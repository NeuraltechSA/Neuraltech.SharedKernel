namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

public sealed record Order
{
    private readonly OrderBy _orderBy;
    private readonly OrderType _orderType;

    public Order(string orderBy, OrderTypes orderType){
        _orderBy =  OrderBy.Create(orderBy);
        _orderType = OrderType.Create(orderType);
    }

    public static Order Create(string orderBy, OrderTypes orderType) => new Order(orderBy, orderType);

    public string GetOrderBy() => _orderBy.Value;
    public OrderTypes GetOrderType() => _orderType.Value;
};
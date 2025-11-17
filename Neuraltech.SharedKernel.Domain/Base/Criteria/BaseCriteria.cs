using Neuraltech.SharedKernel.Domain.Base.Criteria.Fields;
using Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;
using Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

namespace Neuraltech.SharedKernel.Domain.Base.Criteria;

public class BaseCriteria<T> where T : BaseCriteria<T>
{
    protected Pagination? Pagination { get; set; }
    protected Filters Filters { get; set; }
    protected Orders Orders { get; set; }

    public BaseCriteria(Pagination? pagination = null, Filters? filters = null, Orders? orders = null)
    {
        Filters = filters ?? new Filters([]);
        Orders = orders ?? new Orders([]);
        Pagination = pagination;
    }

    public static T Create()
    {
        return Activator.CreateInstance<T>();
    }

    public T Paginate(long page, long pageSize)
    {

        Pagination = new Pagination(pageSize, page);
        return (T)this;
    }

    public bool HasPagination => Pagination != null;

    public long? GetPageSize() => Pagination?.Size;

    public long? GetPageNumber() => Pagination?.Page;

    public long? GetPageNumberFromZero() => Pagination?.Page - 1;

    public T AddFilter(string field, FilterOperators op, object? value)
    {
        var newFilter = Filter.Create(field, op, value);
        var newFilters = new List<Filter>(Filters.Value) { newFilter };
        Filters = new Filters(newFilters);
        return (T)this;
    }

    public T AddOrder(string orderBy, OrderTypes orderType)
    {
        var newOrder = Order.Create(orderBy, orderType);
        var newOrders = new List<Order>(Orders.Value) { newOrder };
        Orders = new Orders(newOrders);
        return (T)this;
    }
    
    public bool HasOrder => Orders.Value.Any();

    public List<Filter> GetFilters() => Filters.Value;

    public List<Order> GetOrders() => Orders.Value;

}
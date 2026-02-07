using System;
using System.Collections.Generic;
using System.Linq;
using Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;
using Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

namespace Neuraltech.SharedKernel.Domain.Base.Criteria;

public class BaseCriteria<T> : IPaginable<T>, IOrderable<T>
    where T : BaseCriteria<T>
{
    protected Pagination? Pagination { get; set; }
    protected Filters Filters { get; set; }
    protected Orders Orders { get; set; }

    public BaseCriteria(
        Pagination? pagination = null,
        Filters? filters = null,
        Orders? orders = null
    )
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

    public long? GetPageSize() => Pagination?.Size;

    public long? GetPageNumber() => Pagination?.Page;

    public long? GetPageNumberFromZero() => Pagination?.Page - 1;

    public T AddFilter(string fieldName, FilterOperators op, object? value)
    {
        var newFilter = Filter.Create(fieldName, op, value);
        var newFilters = new List<Filter>(Filters.Value) { newFilter };
        Filters = new Filters(newFilters);
        return (T)this;
    }

    public T AddOrder(string propertyName, OrderTypes orderType)
    {
        var newOrder = Order.Create(propertyName, orderType);
        var newOrders = new List<Order>(Orders.Value) { newOrder };
        Orders = new Orders(newOrders);
        return (T)this;
    }

    public List<Filter> GetFilters() => Filters.Value;

    public List<Order> GetOrders() => Orders.Value;

    public bool HasPagination()
    {
        return Pagination != null;
    }

    public bool HasOrder()
    {
        return Orders.Value.Any();
    }

    public override string ToString()
    {
        var parts = new List<string>();

        var f = Filters?.ToString() ?? "";
        if (!string.IsNullOrEmpty(f))
            parts.Add(f);

        var o = Orders?.ToString() ?? "";
        if (!string.IsNullOrEmpty(o))
            parts.Add(o);

        var p = Pagination?.ToString() ?? "";
        if (!string.IsNullOrEmpty(p))
            parts.Add(p);

        return parts.Any() ? string.Join("|", parts) : "none";
    }
}

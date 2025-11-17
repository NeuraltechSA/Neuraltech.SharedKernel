using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;
using Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;
using System.Linq.Dynamic.Core;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;

public class EFCoreCriteriaConverter
{
    private string ParseFilter(Filter filter)
    {
        return filter.GetOperator() switch
        {
            FilterOperators.EQ => $"{filter.GetField()} == @0",
            FilterOperators.NEQ => $"{filter.GetField()} != @0",
            FilterOperators.GT => $"{filter.GetField()} > @0",
            FilterOperators.LT => $"{filter.GetField()} < @0",
            FilterOperators.GTE => $"{filter.GetField()} >= @0",
            FilterOperators.LTE => $"{filter.GetField()} <= @0",
            FilterOperators.CONTAINS => $"{filter.GetField()}.Contains(@0)",
            FilterOperators.NOT_CONTAINS => $"{filter.GetField()}.NotContains(@0)",
            FilterOperators.STARTS_WITH => $"{filter.GetField()}.StartsWith(@0)",
            FilterOperators.ENDS_WITH => $"{filter.GetField()}.EndsWith(@0)",
            FilterOperators.IN => "@0.Contains(" + filter.GetField() + ")",
            FilterOperators.NOT_IN => "!@0.Contains(" + filter.GetField() + ")",
            _ => throw new Exception("Invalid operator")
        };
    }

    private string ParseOrders(List<Order> orders)
    {
        return string.Join(", ", orders.Select(order => $"{order.GetOrderBy()} {(order.GetOrderType() == OrderTypes.ASC ? "asc" : "desc")}"));
    }

    private IQueryable<TModel> ApplyFilters<TModel,TCriteria>(TCriteria criteria, IQueryable<TModel> query) where TCriteria : BaseCriteria<TCriteria>
    {
        foreach (var filter in criteria.GetFilters())
        {
            query = query.Where(ParseFilter(filter), filter.GetValue());
        }
        return query;
    }

    private IQueryable<TModel> ApplyOrders<TModel, TCriteria>(TCriteria criteria, IQueryable<TModel> query) where TCriteria : BaseCriteria<TCriteria>
    {
        if (!criteria.HasOrder) return query;

        query = query.OrderBy(ParseOrders(criteria.GetOrders()));
        return query;
    }

    private IQueryable<TModel> ApplyPagination<TModel, TCriteria>(TCriteria criteria, IQueryable<TModel> query) where TCriteria : BaseCriteria<TCriteria>
    {
        if (!criteria.HasPagination) return query;
        
        var pageNumber = (int)criteria.GetPageNumberFromZero()!;
        var pageSize = (int)criteria.GetPageSize()!;
        query = query.Skip(pageNumber * pageSize).Take(pageSize);
        return query;
    }
    public IQueryable<TModel> Apply<TModel, TCriteria>(TCriteria criteria, IQueryable<TModel> query) where TCriteria : BaseCriteria<TCriteria>
    {
        query = ApplyFilters(criteria, query);
        query = ApplyOrders(criteria, query);
        query = ApplyPagination(criteria, query);

        return query;
    }
}
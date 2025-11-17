namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;

public sealed class Filter
{
    private readonly FilterField _field;
    private readonly FilterOperator _operator;
    private readonly FilterValue _value;

    public Filter(string field, FilterOperators @operator ,object? value)
    {
        _field = new FilterField(field);
        _operator = new FilterOperator(@operator);
        _value = new FilterValue(value);
    }

    public static Filter Create(string field, FilterOperators @operator, object? value) => new Filter(field, @operator, value);

    public object? GetValue() => _value.Value;
    public string GetField() => _field.Value;
    public FilterOperators GetOperator() => _operator.Value;
}
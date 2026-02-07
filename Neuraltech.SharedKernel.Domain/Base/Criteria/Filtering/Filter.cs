namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;

public sealed class Filter
{
    private readonly FilterField _field;
    private readonly FilterOperator _operator;
    private readonly FilterValue _value;

    private Filter(FilterField field, FilterOperator @operator, FilterValue value)
    {
        _field = field;
        _operator = @operator;
        _value = value;
    }

    public static Filter Create(string fieldName, FilterOperators @operator, object? value)
    {
        var field = FilterField.Create(fieldName);
        return new Filter(field, new FilterOperator(@operator), new FilterValue(value));
    }

    public object? GetValue() => _value.Value;

    public string GetField() => _field.Value;

    public FilterOperators GetOperator() => _operator.Value;

    public override string ToString() => $"{_field}_{_operator}_{_value}";
}

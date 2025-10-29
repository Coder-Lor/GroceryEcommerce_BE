using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Application.Common;

public class FieldMapping
{
    public string FieldName { get; set; } = string.Empty;
    public Type FieldType { get; set; } = typeof(string);
    public bool IsSearchable { get; set; } = true;
    public bool IsSortable { get; set; } = true;
    public bool IsFilterable { get; set; } = true;
}

public class FilterCriteria : IValidatableObject
{
    public string FieldName { get; set; } = string.Empty;
    public object? Value { get; set; }
    public FilterOperator Operator { get; set; } = FilterOperator.Equals;

    public FilterCriteria() {}
    public FilterCriteria(string fieldName, object? value, FilterOperator op = FilterOperator.Equals)
    {
        FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        Value = value;
        Operator = op;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var requiresValue = Operator is
            FilterOperator.Equals or
            FilterOperator.NotEquals or
            FilterOperator.Contains or
            FilterOperator.NotContains or
            FilterOperator.StartsWith or
            FilterOperator.EndsWith or
            FilterOperator.GreaterThan or
            FilterOperator.GreaterThanOrEqual or
            FilterOperator.LessThan or
            FilterOperator.LessThanOrEqual or
            FilterOperator.In or
            FilterOperator.NotIn;

        if (requiresValue && Value is null)
        {
            yield return new ValidationResult("The Value field is required for this operator.", new[] { nameof(Value) });
        }
    }
}

public enum FilterOperator
{
    Equals,
    NotEquals,
    Contains,
    NotContains,
    StartsWith,
    EndsWith,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    In,
    NotIn,
    IsNull,
    IsNotNull
}
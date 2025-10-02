namespace GroceryEcommerce.Application.Common;

// Making SearchableField abstract is a breaking change that may affect existing implementations and doesn't appear to be necessary based on its usage.
public abstract class SearchableField(string fieldName, Type fieldType, bool isSearchable, bool isSortable = true)
{
    public string FieldName { get; init; } = fieldName;
    public Type FieldType { get; init; } = fieldType;
    public bool IsSearchable { get; init; } = isSearchable;
    public bool IsSortable { get; init; } = isSortable;
}
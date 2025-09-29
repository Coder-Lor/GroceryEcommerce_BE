namespace GroceryEcommerce.Application.Common;

public class SearchableField
{
    public SearchableField(string fieldName, Type fieldType, bool isSearchable, bool isSortable = true)
    {
        FieldName = fieldName;
        FieldType = fieldType;
        IsSearchable = isSearchable;
        IsSortable = isSortable;
    }

    public string FieldName { get; init; } = string.Empty;
    public Type FieldType { get; init; } = typeof(string);
    public bool IsSearchable { get; init; } = true;
    public bool IsSortable { get; init; } = true;
}
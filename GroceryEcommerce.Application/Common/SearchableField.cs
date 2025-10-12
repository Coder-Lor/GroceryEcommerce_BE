namespace GroceryEcommerce.Application.Common;

public class SearchableField(string fieldName, Type fieldType, bool isSearchable = true, bool isSortable = true)
{
    public string FieldName { get; init; } = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
    public Type FieldType { get; init; } = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
    public bool IsSearchable { get; init; } = isSearchable;
    public bool IsSortable { get; init; } = isSortable;

    public static SearchableField Create(string fieldName, Type fieldType, bool isSearchable = true, bool isSortable = true)
        => new(fieldName, fieldType, isSearchable, isSortable);
}
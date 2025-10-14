namespace GroceryEcommerce.Application.Common;

public class SearchableField(string fieldName, Type fieldType, bool isSearchable = true, bool isSortable = true, bool isFilterable = true)
{
    public string FieldName { get; set; } = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
    public Type FieldType { get; set; } = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
    public bool IsSearchable { get; set; } = isSearchable;
    public bool IsSortable { get; set; } = isSortable;
    public bool IsFilterable { get; set; } = isFilterable;

    public static SearchableField Create(string fieldName, Type fieldType, bool isSearchable = true, bool isSortable = true, bool isFilterable = true)
        => new(fieldName, fieldType, isSearchable, isSortable, isFilterable);
}
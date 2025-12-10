using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;

internal static class SearchPredicateBuilder
{
    public static IPredicate BuildContainsPredicate(string searchTerm, params EntityField2[] fields)
    {
        if (fields is null || fields.Length == 0)
        {
            throw new ArgumentException("At least one field is required to build a predicate.", nameof(fields));
        }

        var trimmedTerm = (searchTerm ?? string.Empty).Trim();
        var predicate = new PredicateExpression();

        foreach (var field in fields)
        {
            predicate.AddWithOr(CreateContainsPredicate(field, trimmedTerm));
        }

        return predicate;
    }

    private static IPredicate CreateContainsPredicate(EntityField2 field, string searchTerm)
    {
        if (field.DataType != typeof(string))
        {
            throw new InvalidOperationException("Contains predicate only supports string fields.");
        }

        var normalizedField = (EntityField2)field.Clone();
        normalizedField.ExpressionToApply = new DbFunctionCall("unaccent(lower({0}))", new object[] { field });

        var normalizedValue = BuildLikePattern(RemoveDiacritics(searchTerm).ToLowerInvariant());

        return normalizedField % normalizedValue;
    }

    private static string BuildLikePattern(string value) => $"%{value}%";

    private static string RemoveDiacritics(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        var normalized = value.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }
}


using System;
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

        var trimmedTerm = searchTerm?.Trim() ?? string.Empty;
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

        return field % BuildLikePattern(searchTerm);
    }

    private static string BuildLikePattern(string value) => $"%{value}%";
}


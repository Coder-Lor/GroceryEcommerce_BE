using System;
using System.Data;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;

internal static class SearchPredicateBuilder
{
    private const string LowerFunctionName = "lower";
    private const string UnaccentFunctionName = "unaccent";

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
            return field.Contains(searchTerm);
        }

        var normalizedField = new EntityField2(
            $"{field.Name}_normalized",
            BuildNormalizedExpression(field),
            typeof(string));

        var parameter = CreateLikeParameter(searchTerm);
        var normalizedValue = BuildNormalizedExpression(parameter);

        return new FieldCompareExpressionPredicate(
            normalizedField,
            ComparisonOperator.Like,
            normalizedValue);
    }

    private static IExpression BuildNormalizedExpression(object source)
    {
        var lowerExpression = new DbFunctionCall(LowerFunctionName, new object[] { source });
        return new DbFunctionCall(UnaccentFunctionName, new object[] { lowerExpression });
    }

    private static ParameterValue CreateLikeParameter(string searchTerm)
    {
        var value = $"%{searchTerm}%";

        return new ParameterValue(
            ParameterDirection.Input,
            value,
            value.Length,
            null,
            null,
            DbType.String,
            true);
    }
}


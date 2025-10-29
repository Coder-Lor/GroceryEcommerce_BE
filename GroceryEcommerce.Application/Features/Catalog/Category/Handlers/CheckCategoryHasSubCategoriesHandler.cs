using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class CheckCategoryHasSubCategoriesHandler(
    ICategoryRepository categoryRepository,
    ILogger<CheckCategoryHasSubCategoriesHandler> logger
) : IRequestHandler<CheckCategoryHasSubCategoriesQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckCategoryHasSubCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if category has subcategories: {CategoryId}", request.CategoryId);

            var result = await categoryRepository.HasSubCategoriesAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to check subcategories for category: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to check subcategories.");
            }

            return Result<bool>.Success(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking subcategories for category: {CategoryId}", request.CategoryId);
            return Result<bool>.Failure("An error occurred while checking subcategories.");
        }
    }
}


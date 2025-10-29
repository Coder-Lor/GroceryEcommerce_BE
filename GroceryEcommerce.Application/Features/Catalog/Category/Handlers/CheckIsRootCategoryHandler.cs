using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class CheckIsRootCategoryHandler(
    ICategoryRepository categoryRepository,
    ILogger<CheckIsRootCategoryHandler> logger
) : IRequestHandler<CheckIsRootCategoryQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckIsRootCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if category is root category: {CategoryId}", request.CategoryId);

            var result = await categoryRepository.IsRootCategoryAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to check if category is root: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to check root category status.");
            }

            return Result<bool>.Success(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category is root: {CategoryId}", request.CategoryId);
            return Result<bool>.Failure("An error occurred while checking root category status.");
        }
    }
}

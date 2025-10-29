using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class CheckCategoryInUseHandler(
    ICategoryRepository categoryRepository,
    ILogger<CheckCategoryInUseHandler> logger
) : IRequestHandler<CheckCategoryInUseQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckCategoryInUseQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if category is in use: {CategoryId}", request.CategoryId);

            var result = await categoryRepository.IsCategoryInUseAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to check if category is in use: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to check category usage.");
            }

            return Result<bool>.Success(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category is in use: {CategoryId}", request.CategoryId);
            return Result<bool>.Failure("An error occurred while checking category usage.");
        }
    }
}


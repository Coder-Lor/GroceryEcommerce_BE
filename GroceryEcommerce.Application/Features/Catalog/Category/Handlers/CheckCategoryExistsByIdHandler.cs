using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Handlers;

public class CheckCategoryExistsByIdHandler(
    ICategoryRepository categoryRepository,
    ILogger<CheckCategoryExistsByIdHandler> logger
) : IRequestHandler<CheckCategoryExistsByIdQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckCategoryExistsByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if category exists by id: {CategoryId}", request.CategoryId);

            var result = await categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to check category existence by id: {CategoryId}", request.CategoryId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to check category existence.");
            }

            return Result<bool>.Success(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking category existence by id: {CategoryId}", request.CategoryId);
            return Result<bool>.Failure("An error occurred while checking category existence.");
        }
    }
}

using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class CheckCategoryExistsHandler(
    ICategoryRepository categoryRepository,
    ILogger<CheckCategoryExistsHandler> logger
) : IRequestHandler<CheckCategoryExistsQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckCategoryExistsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if category exists by name: {Name}", request.Name);

            var result = await categoryRepository.ExistsAsync(request.Name, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to check category existence by name: {Name}", request.Name);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to check category existence.");
            }

            return Result<bool>.Success(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking category existence by name: {Name}", request.Name);
            return Result<bool>.Failure("An error occurred while checking category existence.");
        }
    }
}


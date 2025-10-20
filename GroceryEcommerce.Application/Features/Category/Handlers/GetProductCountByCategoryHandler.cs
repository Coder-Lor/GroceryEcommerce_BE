using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class GetProductCountByCategoryHandler(
    ICategoryRepository categoryRepository,
    ILogger<GetProductCountByCategoryHandler> logger
) : IRequestHandler<GetProductCountByCategoryQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetProductCountByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting product count for category: {CategoryId}", request.CategoryId);

            var result = await categoryRepository.GetProductCountByCategoryAsync(request.CategoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to get product count for category: {CategoryId}", request.CategoryId);
                return Result<int>.Failure(result.ErrorMessage ?? "Failed to get product count.");
            }

            return Result<int>.Success(result.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product count for category: {CategoryId}", request.CategoryId);
            return Result<int>.Failure("An error occurred while retrieving product count.");
        }
    }
}


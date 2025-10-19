using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class CheckProductExistsByCategoryIdHandler(
    IProductRepository repository,
    ILogger<CheckProductExistsByCategoryIdHandler> logger
) : IRequestHandler<CheckProductExistsByCategoryIdQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckProductExistsByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking if products exist for category: {CategoryId}", request.CategoryId);

        var pagedRequest = new PagedRequest
        {
            Page = 1,
            PageSize = 1
        };
        pagedRequest.WithFilter("CategoryId", request.CategoryId);

        var result = await repository.GetByCategoryIdAsync(pagedRequest, request.CategoryId, cancellationToken);
        var exists = result.IsSuccess && result.Data.Items.Any();

        logger.LogInformation("Products exist for category {CategoryId}: {Exists}", request.CategoryId, exists);
        return Result<bool>.Success(exists);
    }
}

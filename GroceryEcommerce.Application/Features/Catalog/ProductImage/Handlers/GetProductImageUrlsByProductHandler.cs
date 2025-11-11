using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductImage.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductImage.Handlers;

public class GetProductImageUrlsByProductHandler(
    IProductImageRepository repository,
    ILogger<GetProductImageUrlsByProductHandler> logger
) : IRequestHandler<GetProductImageUrlsByProductQuery, Result<List<string>>>
{
    public async Task<Result<List<string>>> Handle(GetProductImageUrlsByProductQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product image urls for product {ProductId}", request.ProductId);
        var result = await repository.GetImageUrlsByProductAsync(request.ProductId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<List<string>>.Failure(result.ErrorMessage ?? "Failed to get image urls");
        }
        return Result<List<string>>.Success(result.Data);
    }
}


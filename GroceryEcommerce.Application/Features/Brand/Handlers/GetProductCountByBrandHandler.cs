using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Brand.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Brand.Handlers;

public class GetProductCountByBrandHandler(
    IBrandRepository repository,
    ILogger<GetProductCountByBrandHandler> logger
) : IRequestHandler<GetProductCountByBrandQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetProductCountByBrandQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductCountByBrandQuery for brand: {BrandId}", request.BrandId);

        var productCountResult = await repository.GetProductCountByBrandAsync(request.BrandId, cancellationToken);
        if (!productCountResult.IsSuccess)
        {
            logger.LogWarning("Product count not found for brand: {BrandId}", request.BrandId);
            return Result<int>.Failure("Product count not found");
        }

        return Result<int>.Success(productCountResult.Data);
    }
}
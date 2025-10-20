using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class CheckProductExistsBySkuHandler(
    IProductRepository repository,
    ILogger<CheckProductExistsBySkuHandler> logger
) : IRequestHandler<CheckProductExistsBySkuQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckProductExistsBySkuQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking if product exists with SKU: {Sku}", request.Sku);

        var result = await repository.GetBySkuAsync(request.Sku, cancellationToken);
        var exists = result.IsSuccess && result.Data != null;

        logger.LogInformation("Product with SKU {Sku} exists: {Exists}", request.Sku, exists);
        return Result<bool>.Success(exists);
    }
}

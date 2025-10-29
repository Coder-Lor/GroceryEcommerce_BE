using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class CheckProductExistsBySlugHandler(
    IProductRepository repository,
    ILogger<CheckProductExistsBySlugHandler> logger
) : IRequestHandler<CheckProductExistsBySlugQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckProductExistsBySlugQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking if product exists with slug: {Slug}", request.Slug);

        var result = await repository.GetBySlugAsync(request.Slug, cancellationToken);
        var exists = result.IsSuccess && result.Data != null;

        logger.LogInformation("Product with slug {Slug} exists: {Exists}", request.Slug, exists);
        return Result<bool>.Success(exists);
    }
}

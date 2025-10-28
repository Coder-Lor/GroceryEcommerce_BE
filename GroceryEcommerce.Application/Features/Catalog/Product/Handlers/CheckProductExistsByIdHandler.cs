using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class CheckProductExistsByIdHandler(
    IProductRepository repository,
    ILogger<CheckProductExistsByIdHandler> logger
) : IRequestHandler<CheckProductExistsByIdQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckProductExistsByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking if product exists with ID: {ProductId}", request.ProductId);

        var result = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        var exists = result is { IsSuccess: true, Data: not null };

        logger.LogInformation("Product with ID {ProductId} exists: {Exists}", request.ProductId, exists);
        return Result<bool>.Success(exists);
    }
}

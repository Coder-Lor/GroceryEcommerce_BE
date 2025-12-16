using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class DeleteShopCommandHandler(
    IShopRepository repository,
    ILogger<DeleteShopCommandHandler> logger
) : IRequestHandler<DeleteShopCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteShopCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting shop: {ShopId}", request.ShopId);

        var shopResult = await repository.GetByIdAsync(request.ShopId, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found: {ShopId}", request.ShopId);
            return Result<bool>.Failure("Shop not found");
        }

        var deleteResult = await repository.DeleteAsync(request.ShopId, cancellationToken);
        if (deleteResult.IsSuccess)
        {
            logger.LogInformation("Shop deleted successfully: {ShopId}", request.ShopId);
            return Result<bool>.Success(true);
        }

        logger.LogError("Failed to delete shop: {ShopId}", request.ShopId);
        return Result<bool>.Failure("Failed to delete shop");
    }
}



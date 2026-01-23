using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class AcceptShopCommandHandler(
    IShopRepository repository,
    ILogger<AcceptShopCommandHandler> logger
) : IRequestHandler<AcceptShopCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AcceptShopCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Accepting shop: {ShopId}", request.ShopId);

        var shopResult = await repository.GetByIdAsync(request.ShopId, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found: {ShopId}", request.ShopId);
            return Result<bool>.Failure("Shop not found");
        }

        var shop = shopResult.Data;
        shop.IsAccepted = true;
        shop.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.UpdateAsync(shop, cancellationToken);
        if (updated.IsSuccess)
        {
            logger.LogInformation("Shop accepted successfully: {ShopId}", request.ShopId);
            return Result<bool>.Success(true);
        }

        logger.LogError("Failed to accept shop: {ShopId}", request.ShopId);
        return Result<bool>.Failure("Failed to accept shop");
    }
}

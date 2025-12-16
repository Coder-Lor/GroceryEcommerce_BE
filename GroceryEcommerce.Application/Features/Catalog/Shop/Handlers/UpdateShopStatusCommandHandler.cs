using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class UpdateShopStatusCommandHandler(
    IShopRepository repository,
    ICurrentUserService currentUserService,
    ILogger<UpdateShopStatusCommandHandler> logger
) : IRequestHandler<UpdateShopStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateShopStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating shop status: {ShopId} to {Status}", request.ShopId, request.Status);

        var shopResult = await repository.GetByIdAsync(request.ShopId, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found: {ShopId}", request.ShopId);
            return Result<bool>.Failure("Shop not found");
        }

        var shop = shopResult.Data;
        shop.Status = request.Status;
        shop.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.UpdateAsync(shop, cancellationToken);
        if (updated.IsSuccess)
        {
            logger.LogInformation("Shop status updated successfully: {ShopId}", request.ShopId);
            return Result<bool>.Success(true);
        }

        logger.LogError("Failed to update shop status: {ShopId}", request.ShopId);
        return Result<bool>.Failure("Failed to update shop status");
    }
}



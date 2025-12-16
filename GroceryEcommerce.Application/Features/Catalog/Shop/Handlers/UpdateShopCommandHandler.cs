using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class UpdateShopCommandHandler(
    IShopRepository repository,
    ICurrentUserService currentUserService,
    ILogger<UpdateShopCommandHandler> logger
) : IRequestHandler<UpdateShopCommand, Result<UpdateShopResponse>>
{
    public async Task<Result<UpdateShopResponse>> Handle(UpdateShopCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating shop: {ShopId}", request.ShopId);

        var shopResult = await repository.GetByIdAsync(request.ShopId, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found: {ShopId}", request.ShopId);
            return Result<UpdateShopResponse>.Failure("Shop not found");
        }

        var shop = shopResult.Data;

        shop.Name = request.Name;
        shop.Slug = request.Slug ?? shop.Slug;
        shop.Description = request.Description;
        shop.LogoUrl = request.LogoUrl;
        shop.Status = request.Status;
        shop.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.UpdateAsync(shop, cancellationToken);
        if (!updated.IsSuccess)
        {
            logger.LogError("Failed to update shop: {ShopId}", request.ShopId);
            return Result<UpdateShopResponse>.Failure("Failed to update shop");
        }

        logger.LogInformation("Shop updated successfully: {ShopId}", request.ShopId);

        var response = new UpdateShopResponse
        {
            ShopId = shop.ShopId,
            Name = shop.Name,
            Slug = shop.Slug,
            Description = shop.Description,
            LogoUrl = shop.LogoUrl,
            Status = shop.Status,
            OwnerUserId = shop.OwnerUserId,
            CreatedAt = shop.CreatedAt,
            UpdatedAt = shop.UpdatedAt
        };

        return Result<UpdateShopResponse>.Success(response);
    }
}



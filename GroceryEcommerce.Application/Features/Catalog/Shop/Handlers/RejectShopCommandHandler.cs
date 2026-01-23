using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class RejectShopCommandHandler(
    IShopRepository repository,
    ILogger<RejectShopCommandHandler> logger
) : IRequestHandler<RejectShopCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RejectShopCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rejecting shop: {ShopId}", request.ShopId);

        var shopResult = await repository.GetByIdAsync(request.ShopId, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found: {ShopId}", request.ShopId);
            return Result<bool>.Failure("Shop not found");
        }

        // Xóa shop khi từ chối
        var deleteResult = await repository.DeleteAsync(request.ShopId, cancellationToken);
        if (deleteResult.IsSuccess)
        {
            logger.LogInformation("Shop rejected and deleted successfully: {ShopId}", request.ShopId);
            return Result<bool>.Success(true);
        }

        logger.LogError("Failed to reject shop: {ShopId}", request.ShopId);
        return Result<bool>.Failure("Failed to reject shop");
    }
}

using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Handlers;

public class DeletePurchaseOrderHandler(
    IPurchaseOrderRepository repository,
    ILogger<DeletePurchaseOrderHandler> logger
) : IRequestHandler<DeletePurchaseOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await repository.DeleteAsync(request.PurchaseOrderId, cancellationToken);
        if (!deleteResult.IsSuccess)
        {
            return Result<bool>.Failure(deleteResult.ErrorMessage ?? "Failed to delete purchase order");
        }
        return Result<bool>.Success(true);
    }
}



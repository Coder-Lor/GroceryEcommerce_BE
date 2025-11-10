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
        var deleteSuccess = await repository.DeleteAsync(request.PurchaseOrderId, cancellationToken);
        if (!deleteSuccess)
        {
            return Result<bool>.Failure("Failed to delete purchase order");
        }
        return Result<bool>.Success(true);
    }
}



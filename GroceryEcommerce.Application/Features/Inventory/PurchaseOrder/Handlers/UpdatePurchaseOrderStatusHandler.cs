using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Handlers;

public class UpdatePurchaseOrderStatusHandler(
    IPurchaseOrderRepository repository,
    ILogger<UpdatePurchaseOrderStatusHandler> logger
) : IRequestHandler<UpdatePurchaseOrderStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        return await repository.UpdateStatusAsync(request.PurchaseOrderId, request.Status, cancellationToken);
    }
}



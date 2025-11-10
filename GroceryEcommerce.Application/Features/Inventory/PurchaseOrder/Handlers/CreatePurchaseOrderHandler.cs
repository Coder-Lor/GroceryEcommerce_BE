using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Handlers;

public class CreatePurchaseOrderHandler(
    IPurchaseOrderRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ILogger<CreatePurchaseOrderHandler> logger
) : IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating purchase order");

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Result<PurchaseOrderDto>.Failure("Unable to identify current user");
            }

            var orderNumberResult = await repository.GenerateOrderNumberAsync(cancellationToken);
            if (!orderNumberResult.IsSuccess)
            {
                return Result<PurchaseOrderDto>.Failure("Failed to generate order number");
            }

            var purchaseOrder = new Domain.Entities.Inventory.PurchaseOrder
            {
                PurchaseOrderId = Guid.NewGuid(),
                OrderNumber = orderNumberResult.Data!,
                OrderDate = DateTime.UtcNow,
                ExpectedDate = request.ExpectedDate,
                Status = 1,
                TotalAmount = request.Items.Sum(i => i.UnitCost * i.Quantity),
                CreatedBy = currentUserId.Value,
                CreatedAt = DateTime.UtcNow
            };

            var createSuccess = await repository.CreateAsync(purchaseOrder, cancellationToken);
            if (!createSuccess)
            {
                return Result<PurchaseOrderDto>.Failure("Failed to create purchase order");
            }

            // Get the created purchase order to include all data
            var getResult = await repository.GetByIdAsync(purchaseOrder.PurchaseOrderId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Data == null)
            {
                return Result<PurchaseOrderDto>.Failure("Purchase order was created but could not be retrieved");
            }

            var dto = mapper.Map<PurchaseOrderDto>(getResult.Data);
            return Result<PurchaseOrderDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<PurchaseOrderDto>.Failure("Faield to create purchase product " + ex.Message);
        }
    }
}



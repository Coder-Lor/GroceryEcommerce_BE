using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Handlers;

public class GetPurchaseOrderByIdHandler(
    IPurchaseOrderRepository repository,
    IMapper mapper,
    ILogger<GetPurchaseOrderByIdHandler> logger
) : IRequestHandler<GetPurchaseOrderByIdQuery, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByIdAsync(request.PurchaseOrderId, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<PurchaseOrderDto>.Failure("Purchase order not found");
        }

        var dto = mapper.Map<PurchaseOrderDto>(result.Data);
        return Result<PurchaseOrderDto>.Success(dto);
    }
}



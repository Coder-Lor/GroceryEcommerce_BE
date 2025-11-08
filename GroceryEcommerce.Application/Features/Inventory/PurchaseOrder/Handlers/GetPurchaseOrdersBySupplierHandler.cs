using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Handlers;

public class GetPurchaseOrdersBySupplierHandler(
    IPurchaseOrderRepository repository,
    IMapper mapper,
    ILogger<GetPurchaseOrdersBySupplierHandler> logger
) : IRequestHandler<GetPurchaseOrdersBySupplierQuery, Result<PagedResult<PurchaseOrderDto>>>
{
    public async Task<Result<PagedResult<PurchaseOrderDto>>> Handle(GetPurchaseOrdersBySupplierQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetBySupplierIdAsync(request.SupplierId, request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<PagedResult<PurchaseOrderDto>>.Failure(result.ErrorMessage);
        }

        var dtoResult = mapper.Map<PagedResult<PurchaseOrderDto>>(result.Data);
        return Result<PagedResult<PurchaseOrderDto>>.Success(dtoResult);
    }
}



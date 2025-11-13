using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Handlers;

public class GetPurchaseOrdersPagingHandler(
    IPurchaseOrderRepository repository,
    IMapper mapper,
    ILogger<GetPurchaseOrdersPagingHandler> logger
) : IRequestHandler<GetPurchaseOrdersPagingQuery, Result<PagedResult<PurchaseOrderDto>>>
{
    public async Task<Result<PagedResult<PurchaseOrderDto>>> Handle(GetPurchaseOrdersPagingQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<PagedResult<PurchaseOrderDto>>.Failure(result.ErrorMessage);
        }

        var dtoItems = mapper.Map<List<PurchaseOrderDto>>(result.Data.Items);
        var dtoResult = new PagedResult<PurchaseOrderDto>(
            dtoItems,
            result.Data.TotalCount,
            result.Data.Page,
            result.Data.PageSize
        );

        return Result<PagedResult<PurchaseOrderDto>>.Success(dtoResult);
    }
}



using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Handlers;

public class GetWarehousesPagingHandler(
    IWarehouseRepository repository,
    IMapper mapper,
    ILogger<GetWarehousesPagingHandler> logger
) : IRequestHandler<GetWarehousesPagingQuery, Result<PagedResult<WarehouseDto>>>
{
    public async Task<Result<PagedResult<WarehouseDto>>> Handle(GetWarehousesPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting warehouses paging: Page {Page}, PageSize {PageSize}", 
            request.Request.Page, request.Request.PageSize);

        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            logger.LogWarning("Failed to get warehouses");
            return Result<PagedResult<WarehouseDto>>.Failure(result.ErrorMessage);
        }

        var dtoResult = mapper.Map<PagedResult<WarehouseDto>>(result.Data);
        return Result<PagedResult<WarehouseDto>>.Success(dtoResult);
    }
}



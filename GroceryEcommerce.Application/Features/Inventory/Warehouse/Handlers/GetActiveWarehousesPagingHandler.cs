using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Handlers;

public class GetActiveWarehousesPagingHandler(
    IWarehouseRepository repository,
    IMapper mapper,
    ILogger<GetActiveWarehousesPagingHandler> logger
) : IRequestHandler<GetActiveWarehousesPagingQuery, Result<PagedResult<WarehouseDto>>>
{
    public async Task<Result<PagedResult<WarehouseDto>>> Handle(GetActiveWarehousesPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting active warehouses paging: Page {Page}, PageSize {PageSize}", 
            request.Request.Page, request.Request.PageSize);

        var result = await repository.GetActiveWarehousesAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            logger.LogWarning("Failed to get active warehouses");
            return Result<PagedResult<WarehouseDto>>.Failure(result.ErrorMessage);
        }

        var dtoResult = mapper.Map<PagedResult<WarehouseDto>>(result.Data);
        return Result<PagedResult<WarehouseDto>>.Success(dtoResult);
    }
}



using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Handlers;

public class GetStockMovementsPagingHandler(
    IStockMovementRepository repository,
    IMapper mapper,
    ILogger<GetStockMovementsPagingHandler> logger
) : IRequestHandler<GetStockMovementsPagingQuery, Result<PagedResult<StockMovementDto>>>
{
    public async Task<Result<PagedResult<StockMovementDto>>> Handle(GetStockMovementsPagingQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<PagedResult<StockMovementDto>>.Failure(result.ErrorMessage);
        }

        var dtoResult = mapper.Map<PagedResult<StockMovementDto>>(result.Data);
        return Result<PagedResult<StockMovementDto>>.Success(dtoResult);
    }
}



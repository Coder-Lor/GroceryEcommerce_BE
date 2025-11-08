using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Handlers;

public class GetStockMovementsByProductHandler(
    IStockMovementRepository repository,
    IMapper mapper,
    ILogger<GetStockMovementsByProductHandler> logger
) : IRequestHandler<GetStockMovementsByProductQuery, Result<PagedResult<StockMovementDto>>>
{
    public async Task<Result<PagedResult<StockMovementDto>>> Handle(GetStockMovementsByProductQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByProductIdAsync(request.ProductId, request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<PagedResult<StockMovementDto>>.Failure(result.ErrorMessage);
        }

        var dtoResult = mapper.Map<PagedResult<StockMovementDto>>(result.Data);
        return Result<PagedResult<StockMovementDto>>.Success(dtoResult);
    }
}



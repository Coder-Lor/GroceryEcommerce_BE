using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Handlers;

public class GetStockMovementByIdHandler(
    IStockMovementRepository repository,
    IMapper mapper,
    ILogger<GetStockMovementByIdHandler> logger
) : IRequestHandler<GetStockMovementByIdQuery, Result<StockMovementDto>>
{
    public async Task<Result<StockMovementDto>> Handle(GetStockMovementByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByIdAsync(request.MovementId, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<StockMovementDto>.Failure("Stock movement not found");
        }

        var dto = mapper.Map<StockMovementDto>(result.Data);
        return Result<StockMovementDto>.Success(dto);
    }
}



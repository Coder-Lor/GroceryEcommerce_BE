using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Handlers;

public class GetOrderRefundByIdHandler(
    IMapper mapper,
    IOrderRefundRepository repository,
    ILogger<GetOrderRefundByIdHandler> logger
) : IRequestHandler<GetOrderRefundByIdQuery, Result<OrderRefundDto>>
{
    public async Task<Result<OrderRefundDto>> Handle(GetOrderRefundByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order refund: {RefundId}", request.RefundId);

            var result = await repository.GetByIdAsync(request.RefundId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<OrderRefundDto>.Failure("Order refund not found.");
            }

            var response = mapper.Map<OrderRefundDto>(result.Data);
            return Result<OrderRefundDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order refund: {RefundId}", request.RefundId);
            return Result<OrderRefundDto>.Failure("An error occurred while retrieving order refund.");
        }
    }
}


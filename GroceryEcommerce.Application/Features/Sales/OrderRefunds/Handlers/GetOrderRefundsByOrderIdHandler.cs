using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Handlers;

public class GetOrderRefundsByOrderIdHandler(
    IMapper mapper,
    IOrderRefundRepository repository,
    ILogger<GetOrderRefundsByOrderIdHandler> logger
) : IRequestHandler<GetOrderRefundsByOrderIdQuery, Result<List<OrderRefundDto>>>
{
    public async Task<Result<List<OrderRefundDto>>> Handle(GetOrderRefundsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order refunds for order: {OrderId}", request.OrderId);

            var result = await repository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<List<OrderRefundDto>>.Failure(result.ErrorMessage ?? "Failed to get order refunds.");
            }

            var response = mapper.Map<List<OrderRefundDto>>(result.Data);
            return Result<List<OrderRefundDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order refunds for order: {OrderId}", request.OrderId);
            return Result<List<OrderRefundDto>>.Failure("An error occurred while retrieving order refunds.");
        }
    }
}


using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class GetOrderPaymentsByOrderIdHandler(
    IMapper mapper,
    IOrderPaymentRepository repository,
    ILogger<GetOrderPaymentsByOrderIdHandler> logger
) : IRequestHandler<GetOrderPaymentsByOrderIdQuery, Result<List<OrderPaymentDto>>>
{
    public async Task<Result<List<OrderPaymentDto>>> Handle(GetOrderPaymentsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order payments for order: {OrderId}", request.OrderId);

            var result = await repository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<List<OrderPaymentDto>>.Failure(result.ErrorMessage ?? "Failed to get order payments.");
            }

            var response = mapper.Map<List<OrderPaymentDto>>(result.Data);
            return Result<List<OrderPaymentDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order payments for order: {OrderId}", request.OrderId);
            return Result<List<OrderPaymentDto>>.Failure("An error occurred while retrieving order payments.");
        }
    }
}


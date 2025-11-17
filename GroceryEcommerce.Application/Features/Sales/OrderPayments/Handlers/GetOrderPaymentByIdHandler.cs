using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class GetOrderPaymentByIdHandler(
    IMapper mapper,
    IOrderPaymentRepository repository,
    ILogger<GetOrderPaymentByIdHandler> logger
) : IRequestHandler<GetOrderPaymentByIdQuery, Result<OrderPaymentDto>>
{
    public async Task<Result<OrderPaymentDto>> Handle(GetOrderPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order payment: {PaymentId}", request.PaymentId);

            var result = await repository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<OrderPaymentDto>.Failure("Order payment not found.");
            }

            var response = mapper.Map<OrderPaymentDto>(result.Data);
            return Result<OrderPaymentDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order payment: {PaymentId}", request.PaymentId);
            return Result<OrderPaymentDto>.Failure("An error occurred while retrieving order payment.");
        }
    }
}


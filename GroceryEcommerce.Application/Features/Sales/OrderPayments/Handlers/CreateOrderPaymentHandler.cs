using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class CreateOrderPaymentHandler(
    IMapper mapper,
    IOrderPaymentRepository repository,
    ILogger<CreateOrderPaymentHandler> logger
) : IRequestHandler<CreateOrderPaymentCommand, Result<OrderPaymentDto>>
{
    public async Task<Result<OrderPaymentDto>> Handle(CreateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order payment for order: {OrderId}", request.Request.OrderId);

            var orderPayment = mapper.Map<OrderPayment>(request.Request);
            orderPayment.PaymentId = Guid.NewGuid();
            orderPayment.CreatedAt = DateTime.UtcNow;

            var createResult = await repository.CreateAsync(orderPayment, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create order payment");
                return Result<OrderPaymentDto>.Failure(createResult.ErrorMessage ?? "Failed to create order payment.");
            }

            var getResult = await repository.GetByIdAsync(orderPayment.PaymentId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Data is null)
            {
                return Result<OrderPaymentDto>.Failure("Order payment created but could not be retrieved.");
            }

            var response = mapper.Map<OrderPaymentDto>(getResult.Data);
            logger.LogInformation("Order payment created successfully: {PaymentId}", orderPayment.PaymentId);
            return Result<OrderPaymentDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order payment");
            return Result<OrderPaymentDto>.Failure("An error occurred while creating order payment.");
        }
    }
}


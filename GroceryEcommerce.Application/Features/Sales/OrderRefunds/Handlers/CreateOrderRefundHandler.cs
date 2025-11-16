using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Handlers;

public class CreateOrderRefundHandler(
    IMapper mapper,
    IOrderRefundRepository repository,
    ILogger<CreateOrderRefundHandler> logger
) : IRequestHandler<CreateOrderRefundCommand, Result<OrderRefundDto>>
{
    public async Task<Result<OrderRefundDto>> Handle(CreateOrderRefundCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order refund for order: {OrderId}", request.Request.OrderId);

            var orderRefund = mapper.Map<OrderRefund>(request.Request);
            orderRefund.RefundId = Guid.NewGuid();
            orderRefund.Status = 1; // Requested
            orderRefund.RequestedAt = DateTime.UtcNow;
            orderRefund.RequestedBy = request.Request.RequestedBy;

            var createResult = await repository.CreateAsync(orderRefund, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create order refund");
                return Result<OrderRefundDto>.Failure(createResult.ErrorMessage ?? "Failed to create order refund.");
            }

            var getResult = await repository.GetByIdAsync(orderRefund.RefundId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Data is null)
            {
                return Result<OrderRefundDto>.Failure("Order refund created but could not be retrieved.");
            }

            var response = mapper.Map<OrderRefundDto>(getResult.Data);
            logger.LogInformation("Order refund created successfully: {RefundId}", orderRefund.RefundId);
            return Result<OrderRefundDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order refund");
            return Result<OrderRefundDto>.Failure("An error occurred while creating order refund.");
        }
    }
}


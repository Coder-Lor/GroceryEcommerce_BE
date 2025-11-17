using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Commands;

public record CreateOrderStatusHistoryCommand(
    CreateOrderStatusHistoryRequest Request
) : IRequest<Result<OrderStatusHistoryDto>>;


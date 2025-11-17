using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Commands;

public record CreateOrderItemCommand(
    CreateOrderItemRequest Request
) : IRequest<Result<OrderItemDto>>;


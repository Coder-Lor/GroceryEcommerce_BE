using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Queries;

public record GetOrderItemByIdQuery(
    Guid OrderItemId
) : IRequest<Result<OrderItemDto>>;


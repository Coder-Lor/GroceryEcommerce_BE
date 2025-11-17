using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;

public record CreateOrderShipmentCommand(
    CreateOrderShipmentRequest Request
) : IRequest<Result<OrderShipmentDto>>;


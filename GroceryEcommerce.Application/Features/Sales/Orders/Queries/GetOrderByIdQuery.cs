using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Queries;

public record GetOrderByIdQuery(
    Guid OrderId
) : IRequest<Result<OrderDetailDto>>;


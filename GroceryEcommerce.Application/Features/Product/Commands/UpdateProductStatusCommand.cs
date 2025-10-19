using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Product.Commands;

public record UpdateProductStatusCommand(
    Guid ProductId,
    short Status
) : IRequest<Result<UpdateProductStatusResponse>>;
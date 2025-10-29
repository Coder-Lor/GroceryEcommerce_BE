using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

public record GetProductByIdQuery(
    Guid ProductId
) : IRequest<Result<GetProductByIdResponse>>;   
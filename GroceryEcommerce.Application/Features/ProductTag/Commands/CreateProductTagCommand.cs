using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductTag.Commands;

public record CreateProductTagCommand(
    string Name,
    string? Slug,
    string? Description
) : IRequest<Result<CreateProductTagResponse>>;

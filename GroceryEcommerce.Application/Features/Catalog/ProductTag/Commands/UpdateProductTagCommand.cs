using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTag.Commands;

public record UpdateProductTagCommand(
    Guid TagId,
    string Name,
    string? Slug,
    string? Description
) : IRequest<Result<UpdateProductTagResponse>>;

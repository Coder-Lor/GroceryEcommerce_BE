using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Brand.Commands;

public record UpdateBrandCommand(
    Guid BrandId,
    string? Name,
    string? Slug,
    string? Description,
    string? LogoUrl,
    string? Website,
    short? Status
) : IRequest<Result<UpdateBrandResponse>>;

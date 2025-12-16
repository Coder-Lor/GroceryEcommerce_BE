using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Commands;

public record UpdateShopCommand(
    Guid ShopId,
    string Name,
    string? Slug,
    string? Description,
    string? LogoUrl,
    short Status
) : IRequest<Result<UpdateShopResponse>>;



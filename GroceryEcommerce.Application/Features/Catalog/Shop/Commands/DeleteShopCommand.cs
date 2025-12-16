using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Commands;

public record DeleteShopCommand(Guid ShopId) : IRequest<Result<bool>>;



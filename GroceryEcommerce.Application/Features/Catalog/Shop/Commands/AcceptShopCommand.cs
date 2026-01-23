using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Commands;

public record AcceptShopCommand(Guid ShopId) : IRequest<Result<bool>>;

using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Queries;

public record GetShopsPagingQuery(PagedRequest Request) : IRequest<Result<PagedResult<ShopDto>>>;



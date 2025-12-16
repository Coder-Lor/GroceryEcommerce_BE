using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Queries;

public record GetProductsByShopPagingQuery(
    Guid ShopId,
    PagedRequest Request
) : IRequest<Result<PagedResult<ProductBaseResponse>>>;



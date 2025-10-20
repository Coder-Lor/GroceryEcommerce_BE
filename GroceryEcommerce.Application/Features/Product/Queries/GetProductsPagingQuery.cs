using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Product.Queries;

public record GetProductsPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<ProductBaseResponse>>>;
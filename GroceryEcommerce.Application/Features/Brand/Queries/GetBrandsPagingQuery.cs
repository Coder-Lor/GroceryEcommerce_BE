using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Brand.Queries;

public record GetBrandsPagingQuery( PagedRequest Request ) : IRequest<Result<PagedResult<BrandDto>>>;
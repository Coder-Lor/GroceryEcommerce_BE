using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Queries;

public record GetActiveBrandsPagingQuery( PagedRequest Request ) : IRequest<Result<PagedResult<BrandDto>>>;
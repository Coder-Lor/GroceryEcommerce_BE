using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Queries;

public record GetProductCountByBrandQuery(Guid BrandId) : IRequest<Result<int>>;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Queries;

public record GetBrandByNameQuery(
    string Name
): IRequest<Result<GetBrandByNameResponse>>;

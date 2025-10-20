using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Brand.Queries;

public record GetBrandBySlugQuery(
    string Slug
): IRequest<Result<GetBrandBySlugResponse>>;
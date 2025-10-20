using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Product.Queries;

public record GetProductBySlugQuery(
    string Slug
) : IRequest<Result<GetProductBySlugResponse>>;
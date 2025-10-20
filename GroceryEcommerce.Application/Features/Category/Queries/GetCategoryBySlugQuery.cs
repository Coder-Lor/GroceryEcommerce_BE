using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Queries;

public record GetCategoryBySlugQuery(
    string Slug
) : IRequest<Result<GetCategoryBySlugResponse>>;

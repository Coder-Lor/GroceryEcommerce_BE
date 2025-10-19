using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Queries;

public record SearchCategoriesByNameQuery(
    string SearchTerm
) : IRequest<Result<SearchCategoriesByNameResponse>>;

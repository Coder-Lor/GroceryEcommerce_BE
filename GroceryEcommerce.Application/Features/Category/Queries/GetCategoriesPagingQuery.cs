using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Queries;

public record GetCategoriesPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<CategoryDto>>>;

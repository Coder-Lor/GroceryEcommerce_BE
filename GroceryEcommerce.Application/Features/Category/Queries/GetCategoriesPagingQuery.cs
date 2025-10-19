using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Queries;

public record GetCategoriesPagingQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    string? SortBy = "Name",
    string? SortDirection = "Asc",
    short? Status = null,
    Guid? ParentCategoryId = null
) : IRequest<Result<PagedResult<CategoryBaseResponse>>>;

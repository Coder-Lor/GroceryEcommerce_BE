using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Product.Queries;

public record GetProductsPagingQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    string? SortBy = "Name",
    string? SortDirection = "Asc",
    short? Status = null,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    bool? IsFeatured = null
) : IRequest<Result<GetProductsPagingResponse>>;
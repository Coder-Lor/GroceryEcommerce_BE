using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Queries;

public record GetCategoryByIdQuery(
    Guid CategoryId
) : IRequest<Result<GetCategoryByIdResponse>>;

using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTag.Queries;

public record GetProductTagByIdQuery(
    Guid TagId
) : IRequest<Result<ProductTagDto>>;

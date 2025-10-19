using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductTag.Queries;

public record GetProductTagByNameQuery(
    string Name
) : IRequest<Result<ProductTagDto>>;

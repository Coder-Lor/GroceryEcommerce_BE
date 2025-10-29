using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Queries;
        
public record CheckProductExistsBySkuQuery(
    string Sku
) : IRequest<Result<bool>>;

public record CheckProductExistsBySlugQuery(
    string Slug
) : IRequest<Result<bool>>;

public record CheckProductExistsByCategoryIdQuery(
    Guid CategoryId
) : IRequest<Result<bool>>;

public record CheckProductExistsByIdQuery(
    Guid ProductId
) : IRequest<Result<bool>>;
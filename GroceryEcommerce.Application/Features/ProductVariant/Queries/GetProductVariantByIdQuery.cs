using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductVariant.Queries;

public record GetProductVariantByIdQuery(
    Guid VariantId
) : IRequest<Result<ProductVariantDto>>;

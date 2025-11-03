using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;

public record CreateProductVariantCommand(
    CreateProductVariantRequest Request
) : IRequest<Result<bool>>;

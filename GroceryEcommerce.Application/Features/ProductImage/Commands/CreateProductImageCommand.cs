using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductImage.Commands;

public record CreateProductImageCommand(
    Guid ProductId,
    string ImageUrl,
    string? AltText,
    bool IsPrimary = false,
    int DisplayOrder = 0
) : IRequest<Result<CreateProductImageResponse>>;

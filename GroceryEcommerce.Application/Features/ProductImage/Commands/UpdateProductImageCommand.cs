using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductImage.Commands;

public record UpdateProductImageCommand(
    Guid ImageId,
    Guid ProductId,
    string ImageUrl,
    string? AltText,
    bool IsPrimary,
    int DisplayOrder
) : IRequest<Result<UpdateProductImageResponse>>;

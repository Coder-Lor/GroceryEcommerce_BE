using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductImage.Queries;

public record GetProductImageByIdQuery(
    Guid ImageId
) : IRequest<Result<ProductImageDto>>;



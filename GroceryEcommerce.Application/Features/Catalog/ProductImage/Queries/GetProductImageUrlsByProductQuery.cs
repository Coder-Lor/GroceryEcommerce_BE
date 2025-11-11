using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductImage.Queries;

public record GetProductImageUrlsByProductQuery(Guid ProductId) : IRequest<Result<List<string>>>;


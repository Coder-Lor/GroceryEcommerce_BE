using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Commands;

public record DeleteBrandCommand(Guid BrandId) : IRequest<Result<bool>>;
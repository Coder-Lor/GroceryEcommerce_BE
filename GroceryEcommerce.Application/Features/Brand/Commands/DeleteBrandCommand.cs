using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Brand.Commands;

public record DeleteBrandCommand(Guid BrandId) : IRequest<Result<bool>>;
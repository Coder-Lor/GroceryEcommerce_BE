using GroceryEcommerce.Application.Common;
using MediatR;

public record UpdateBrandStatusCommand(Guid BrandId, short Status) : IRequest<Result<bool>>;
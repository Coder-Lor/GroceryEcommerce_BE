using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Commands;
public  record UpdateBrandStatusCommand(Guid BrandId, short Status) : IRequest<Result<bool>>;
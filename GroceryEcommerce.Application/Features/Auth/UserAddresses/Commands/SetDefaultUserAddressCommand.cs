using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserAddresses.Commands;

public sealed record SetDefaultUserAddressCommand(Guid UserId, Guid AddressId) : IRequest<Result<bool>>;



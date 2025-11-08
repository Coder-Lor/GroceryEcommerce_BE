using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserAddresses.Queries;

public sealed record GetDefaultUserAddressQuery(Guid UserId) : IRequest<Result<UserAddress?>>;



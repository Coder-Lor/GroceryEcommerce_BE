using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoles.Commands;

public sealed record DeleteUserRoleCommand(Guid RoleId) : IRequest<Result<bool>>;



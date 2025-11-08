using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Commands;

public sealed record RemoveUserRoleCommand(Guid UserId, Guid RoleId) : IRequest<Result<bool>>;



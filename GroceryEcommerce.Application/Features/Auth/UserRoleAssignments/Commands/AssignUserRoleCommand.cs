using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Commands;

public sealed record AssignUserRoleCommand(Guid UserId, Guid RoleId, Guid AssignedBy) : IRequest<Result<bool>>;



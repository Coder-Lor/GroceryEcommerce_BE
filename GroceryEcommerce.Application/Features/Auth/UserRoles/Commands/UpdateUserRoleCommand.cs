using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoles.Commands;

public sealed record UpdateUserRoleCommand : IRequest<Result<bool>>
{
    public required Guid RoleId { get; init; }
    public required string RoleName { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}



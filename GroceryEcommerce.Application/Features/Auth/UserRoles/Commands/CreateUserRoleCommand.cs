using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoles.Commands;

public sealed record CreateUserRoleCommand : IRequest<Result<UserRole>>
{
    public required string RoleName { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}



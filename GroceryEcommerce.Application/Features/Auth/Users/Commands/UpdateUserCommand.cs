using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Users.Commands;

public sealed record UpdateUserCommand : IRequest<Result<bool>>
{
    public required Guid UserId { get; init; }
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string? PasswordHash { get; init; }
}



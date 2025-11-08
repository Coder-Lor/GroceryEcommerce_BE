using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Users.Commands;

public sealed record CreateUserCommand : IRequest<Result<bool>>
{
    public required string Email { get; init; }
    public required string Username { get; init; }
    public required string PasswordHash { get; init; }
}



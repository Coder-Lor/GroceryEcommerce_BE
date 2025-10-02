using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Authentication.Commands;

public sealed record RegisterCommand : IRequest<Result<RegisterResponse>>
{
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed record RegisterResponse
{
    public string UserId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
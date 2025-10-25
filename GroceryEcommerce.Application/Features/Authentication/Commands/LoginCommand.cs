using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Authentication.Commands;

public record LoginCommand : IRequest<Result<LoginResponse>>
{
    public required string EmailOrUsername { get; set; }
    public required string Password { get; set; }
}

public sealed record LoginResponse
{
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
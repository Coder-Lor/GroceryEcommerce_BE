using GroceryEcommerce.Application.Common;
using MediatR;


namespace GroceryEcommerce.Application.Features.Auth.Authentication.Commands
{
    public sealed record RefreshTokenCommand(string RefreshToken)
        :IRequest<Result<RefreshTokenResponse>>;

    public sealed record RefreshTokenResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
}

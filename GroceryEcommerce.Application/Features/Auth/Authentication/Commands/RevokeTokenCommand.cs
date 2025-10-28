using GroceryEcommerce.Application.Common;
using MediatR;


namespace GroceryEcommerce.Application.Features.Auth.Authentication.Commands
{
    public sealed record RevokeTokenCommand(Guid UserId, string? RefreshToken = null) :IRequest<Result>;
}

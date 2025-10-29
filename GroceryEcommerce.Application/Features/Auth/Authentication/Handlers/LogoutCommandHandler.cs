using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Handlers;

public sealed class LogoutCommandHandler(
    ITokenService tokenService,
    ILogger<LogoutCommandHandler> logger)
    : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Logout request received");

        try
        {
            // Revoke refresh token
            await tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            
            logger.LogInformation("User logged out successfully");
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during logout");
            return Result<bool>.Failure("An error occurred during logout");
        }
    }
}

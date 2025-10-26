using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Authentication.Handlers;

public sealed class LogoutCommandHandler(
    ITokenService tokenService,
    ILogger<LogoutCommandHandler> logger)
    : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<LogoutCommandHandler> _logger = logger;

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logout request received");

        try
        {
            // Revoke refresh token
            await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            
            _logger.LogInformation("User logged out successfully");
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during logout");
            return Result<bool>.Failure("An error occurred during logout");
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Handlers;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    ICacheService cacheService,
    IEmailService emailService,
    ILogger<ForgotPasswordCommandHandler> logger)
    : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Forgot password request for email: {Email}", request.Email);
        
        var userResult = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data == null)
        {
            logger.LogWarning("Forgot password: Email not found - {Email}", request.Email);
            return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse
            {
                Message = "If the email exists, a password reset link has been sent"
            });
        }

        var user = userResult.Data;

        var resetToken = Guid.NewGuid().ToString("N")[..6].ToUpper();
        var cacheKey = $"password-reset:{user.UserId}";
        
        await cacheService.SetAsync(cacheKey, new
        {
            Token = resetToken,
            UserId = user.UserId,
            Email = user.Email,
            CreatedAt = DateTime.UtcNow
        }, TimeSpan.FromMinutes(15), cancellationToken);

        try
        {
            await emailService.SendPasswordResetEmailAsync(user.Email, resetToken, cancellationToken);
            logger.LogInformation("Password reset email sent to: {Email}", request.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send password reset email to: {Email}", request.Email);
            return Result<ForgotPasswordResponse>.Failure("Failed to send reset email");
        }
        
        var response = new ForgotPasswordResponse
        {
            Message = "Password reset instructions have been sent to your email",
        };

        return Result<ForgotPasswordResponse>.Success(response);
    }
}

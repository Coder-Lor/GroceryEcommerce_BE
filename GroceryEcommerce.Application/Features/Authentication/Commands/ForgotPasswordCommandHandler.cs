using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Authentication.Commands;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    ICacheService cacheService,
    IEmailService emailService,
    ILogger<ForgotPasswordCommandHandler> logger)
    : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger = logger;
    
    // cần sưửa lại cách lưu reset token

    public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Forgot password request for email: {Email}", request.Email);

        // Get user by email
        var userResult = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data == null)
        {
            // Don't reveal if email exists or not (security)
            _logger.LogWarning("Forgot password: Email not found - {Email}", request.Email);
            return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse
            {
                Message = "If the email exists, a password reset link has been sent"
            });
        }

        var user = userResult.Data;

        // Generate reset token (6-digit code or GUID)
        var resetToken = Guid.NewGuid().ToString("N")[..6].ToUpper(); // 6-character code
        var cacheKey = $"password-reset:{user.UserId}";

        // Store in cache (15 minutes expiry)
        await _cacheService.SetAsync(cacheKey, new
        {
            Token = resetToken,
            UserId = user.UserId,
            Email = user.Email,
            CreatedAt = DateTime.UtcNow
        }, TimeSpan.FromMinutes(15), cancellationToken);

        // Send email (implement IEmailService)
        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken, cancellationToken);
            _logger.LogInformation("Password reset email sent to: {Email}", request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to: {Email}", request.Email);
            return Result<ForgotPasswordResponse>.Failure("Failed to send reset email");
        }

        // Việc trả về mã thông báo đặt lại trong phản hồi là một rủi ro bảo mật. Mã thông báo chỉ nên được gửi qua email và không được hiển thị trong phản hồi API.
        var response = new ForgotPasswordResponse
        {
            Message = "Password reset instructions have been sent to your email",
            ResetToken = resetToken 
        };

        return Result<ForgotPasswordResponse>.Success(response);
    }
}

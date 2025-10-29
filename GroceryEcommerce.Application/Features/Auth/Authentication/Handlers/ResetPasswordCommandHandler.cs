using System.Security.Cryptography;
using System.Text;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Handlers;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    IUnitOfWorkService unitOfWorkService,
    IEmailService emailService,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResponse>>
{
    public async Task<Result<ResetPasswordResponse>> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Reset password attempt for email: {Email}", request.Email);

        var userResult = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data == null)
        {
            logger.LogWarning("Reset password failed: User not found - {Email}", request.Email);
            return Result<ResetPasswordResponse>.Failure("Invalid reset request");
        }

        var user = userResult.Data;
        string newPassword;
        if (string.IsNullOrEmpty(request.NewPassword))
        {
            newPassword = GenerateRandomPassword(12);
            // cần sửa lại sau. không gửi mật khẩu qua email.
            await emailService.SendEmailAsync(user.Email, "Your new password", $"Your new password is: {newPassword}");

        }
        else
        {
            if (request.OldPassword != null &&
                !passwordHashService.VerifyPassword(request.OldPassword, user.PasswordHash))
            {
                return Result<ResetPasswordResponse>.Failure("Old password is incorrect");
            }

            newPassword = request.NewPassword;
        }

        user.PasswordHash = passwordHashService.HashPassword(newPassword);
        await userRepository.UpdateAsync(user, cancellationToken);
        var count =  await unitOfWorkService.SaveChangesAsync(cancellationToken);

        if (count == 0)
        {
            return Result<ResetPasswordResponse>.Failure("Failed to update password");       
        }
        
        return Result<ResetPasswordResponse>.Success(new ResetPasswordResponse
        {
            Message = $"Password reset successful. {count} records updated"
        });
    }


    private string GenerateRandomPassword(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
        StringBuilder bd = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] uintBuffer = new byte[sizeof(uint)];
            while (bd.Length < length) 
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                bd.Append((valid[(int)(num % (uint)valid.Length)]));
            }
        }
        return bd.ToString();
    }
}
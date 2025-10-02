using System.Security.Cryptography;
using System.Text;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Authentication.Commands;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailService _emailService = emailService;   
    private readonly ILogger<ResetPasswordCommandHandler> _logger = logger;
    public async Task<Result<ResetPasswordResponse>> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reset password attempt for email: {Email}", request.Email);

        var userResult = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data == null)
        {
            _logger.LogWarning("Reset password failed: User not found - {Email}", request.Email);
            return Result<ResetPasswordResponse>.Failure("Invalid reset request");
        }

        var user = userResult.Data;
        string newPassword;
        if (string.IsNullOrEmpty(request.NewPassword))
        {
            newPassword = GenerateRandomPassword(12);
            // cần sửa lại sau. không gửi mật khẩu qua email.
            await _emailService.SendEmailAsync(user.Email, "Your new password", $"Your new password is: {newPassword}");

        }
        else
        {
            if (request.OldPassword != null &&
                !_passwordHashService.VerifyPassword(request.OldPassword, user.PasswordHash))
            {
                return Result<ResetPasswordResponse>.Failure("Old password is incorrect");
            }

            newPassword = request.NewPassword;
        }

        user.PasswordHash = _passwordHashService.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        var count =  await _unitOfWork.SaveChangesAsync(cancellationToken);

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
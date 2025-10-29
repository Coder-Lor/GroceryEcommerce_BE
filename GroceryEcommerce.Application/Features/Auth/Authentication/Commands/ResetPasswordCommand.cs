using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Commands;

public sealed record ResetPasswordCommand : IRequest<Result<ResetPasswordResponse>>
{
    public required string Email { get; init; }
    public required string ResetToken { get; init; }
    public string? OldPassword { get; init; }
    public string? NewPassword { get; init; }
}

public sealed record ResetPasswordResponse
{
    public string Message { get; init; } = string.Empty;
}

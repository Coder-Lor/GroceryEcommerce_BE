using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Commands;

public sealed record ForgotPasswordCommand : IRequest<Result<ForgotPasswordResponse>>
{
    public string Email { get; init; } = string.Empty;
}

public sealed record ForgotPasswordResponse
{
    public string Message { get; init; } = string.Empty;
}

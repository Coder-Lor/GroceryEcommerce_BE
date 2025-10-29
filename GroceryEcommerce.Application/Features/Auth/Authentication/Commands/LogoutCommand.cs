using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;

using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Authentication.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;

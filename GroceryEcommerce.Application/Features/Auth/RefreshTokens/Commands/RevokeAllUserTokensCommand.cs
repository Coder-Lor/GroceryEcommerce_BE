using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.RefreshTokens.Commands;

public sealed record RevokeAllUserTokensCommand(Guid UserId) : IRequest<Result<bool>>;



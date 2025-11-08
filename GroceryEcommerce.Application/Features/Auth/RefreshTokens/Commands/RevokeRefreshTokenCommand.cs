using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.RefreshTokens.Commands;

public sealed record RevokeRefreshTokenCommand(Guid TokenId) : IRequest<Result<bool>>;



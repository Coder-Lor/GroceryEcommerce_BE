using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.RefreshTokens.Commands;
using GroceryEcommerce.Application.Features.Auth.RefreshTokens.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.RefreshTokens.Handlers;

public sealed class GetRefreshTokensByUserQueryHandler(IRefreshTokenRepository repository)
    : IRequestHandler<GetRefreshTokensByUserQuery, Result<List<RefreshToken>>>
{
    public Task<Result<List<RefreshToken>>> Handle(GetRefreshTokensByUserQuery request, CancellationToken cancellationToken)
        => repository.GetByUserIdAsync(request.UserId, cancellationToken);
}

public sealed class RevokeRefreshTokenCommandHandler(IRefreshTokenRepository repository)
    : IRequestHandler<RevokeRefreshTokenCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        => repository.RevokeTokenAsync(request.TokenId, cancellationToken);
}

public sealed class RevokeAllUserTokensCommandHandler(IRefreshTokenRepository repository)
    : IRequestHandler<RevokeAllUserTokensCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(RevokeAllUserTokensCommand request, CancellationToken cancellationToken)
        => repository.RevokeAllUserTokensAsync(request.UserId, cancellationToken);
}

public sealed class CleanupExpiredRefreshTokensCommandHandler(IRefreshTokenRepository repository)
    : IRequestHandler<CleanupExpiredRefreshTokensCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(CleanupExpiredRefreshTokensCommand request, CancellationToken cancellationToken)
        => repository.CleanupExpiredTokensAsync(cancellationToken);
}



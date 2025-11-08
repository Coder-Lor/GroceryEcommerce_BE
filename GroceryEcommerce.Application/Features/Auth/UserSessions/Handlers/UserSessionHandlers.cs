using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserSessions.Commands;
using GroceryEcommerce.Application.Features.Auth.UserSessions.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserSessions.Handlers;

public sealed class GetUserSessionsByUserPagedQueryHandler(IUserSessionRepository repository)
    : IRequestHandler<GetUserSessionsByUserPagedQuery, Result<PagedResult<UserSession>>>
{
    public Task<Result<PagedResult<UserSession>>> Handle(GetUserSessionsByUserPagedQuery request, CancellationToken cancellationToken)
        => repository.GetByUserIdAsync(request.Request, request.UserId, cancellationToken);
}

public sealed class GetActiveUserSessionsByUserPagedQueryHandler(IUserSessionRepository repository)
    : IRequestHandler<GetActiveUserSessionsByUserPagedQuery, Result<PagedResult<UserSession>>>
{
    public Task<Result<PagedResult<UserSession>>> Handle(GetActiveUserSessionsByUserPagedQuery request, CancellationToken cancellationToken)
        => repository.GetActiveSessionsByUserIdAsync(request.Request, request.UserId, cancellationToken);
}

public sealed class RevokeUserSessionCommandHandler(IUserSessionRepository repository)
    : IRequestHandler<RevokeUserSessionCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(RevokeUserSessionCommand request, CancellationToken cancellationToken)
        => repository.RevokeSessionAsync(request.SessionId, cancellationToken);
}

public sealed class RevokeAllUserSessionsCommandHandler(IUserSessionRepository repository)
    : IRequestHandler<RevokeAllUserSessionsCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(RevokeAllUserSessionsCommand request, CancellationToken cancellationToken)
        => repository.RevokeAllUserSessionsAsync(request.UserId, cancellationToken);
}



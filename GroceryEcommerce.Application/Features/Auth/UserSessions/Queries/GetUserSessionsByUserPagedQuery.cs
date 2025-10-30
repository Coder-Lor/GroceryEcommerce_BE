using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserSessions.Queries;

public sealed record GetUserSessionsByUserPagedQuery(PagedRequest Request, Guid UserId)
    : IRequest<Result<PagedResult<UserSession>>>;



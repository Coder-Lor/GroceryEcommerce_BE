using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserSessions.Queries;

public sealed record GetActiveUserSessionsByUserPagedQuery(PagedRequest Request, Guid UserId)
    : IRequest<Result<PagedResult<UserSession>>>;



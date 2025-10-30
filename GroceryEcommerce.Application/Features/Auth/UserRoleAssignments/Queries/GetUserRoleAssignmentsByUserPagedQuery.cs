using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Queries;

public sealed record GetUserRoleAssignmentsByUserPagedQuery(PagedRequest Request, Guid UserId)
    : IRequest<Result<PagedResult<UserRoleAssignment>>>;



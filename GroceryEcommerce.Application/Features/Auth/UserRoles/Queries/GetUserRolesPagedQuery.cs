using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoles.Queries;

public sealed record GetUserRolesPagedQuery(PagedRequest Request) : IRequest<Result<PagedResult<UserRole>>>;



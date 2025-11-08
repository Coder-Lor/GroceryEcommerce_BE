using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Queries;

public sealed record GetUserRolesOfUserQuery(Guid UserId) : IRequest<Result<List<string>>>;



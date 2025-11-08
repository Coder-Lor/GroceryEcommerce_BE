using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoles.Queries;

public sealed record GetUserRoleByIdQuery(Guid RoleId) : IRequest<Result<UserRole?>>;



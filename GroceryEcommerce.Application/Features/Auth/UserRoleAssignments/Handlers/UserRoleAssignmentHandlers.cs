using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Commands;
using GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Handlers;

public sealed class AssignUserRoleCommandHandler(IUserRoleAssignmentRepository repository)
    : IRequestHandler<AssignUserRoleCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
        => repository.AssignRoleToUserAsync(request.UserId, request.RoleId, request.AssignedBy, cancellationToken);
}

public sealed class RemoveUserRoleCommandHandler(IUserRoleAssignmentRepository repository)
    : IRequestHandler<RemoveUserRoleCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
        => repository.RemoveRoleFromUserAsync(request.UserId, request.RoleId, cancellationToken);
}

public sealed class GetUserRolesOfUserQueryHandler(IUserRoleAssignmentRepository repository)
    : IRequestHandler<GetUserRolesOfUserQuery, Result<List<string>>>
{
    public Task<Result<List<string>>> Handle(GetUserRolesOfUserQuery request, CancellationToken cancellationToken)
        => repository.GetUserRoleNamesAsync(request.UserId, cancellationToken);
}

public sealed class GetUserRoleAssignmentsByUserPagedQueryHandler(IUserRoleAssignmentRepository repository)
    : IRequestHandler<GetUserRoleAssignmentsByUserPagedQuery, Result<PagedResult<UserRoleAssignment>>>
{
    public Task<Result<PagedResult<UserRoleAssignment>>> Handle(GetUserRoleAssignmentsByUserPagedQuery request, CancellationToken cancellationToken)
        => repository.GetByUserIdAsync(request.Request, request.UserId, cancellationToken);
}



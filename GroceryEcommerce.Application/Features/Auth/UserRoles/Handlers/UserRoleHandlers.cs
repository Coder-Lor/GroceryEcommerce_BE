using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserRoles.Commands;
using GroceryEcommerce.Application.Features.Auth.UserRoles.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserRoles.Handlers;

public sealed class CreateUserRoleCommandHandler(IUserRoleRepository repository)
    : IRequestHandler<CreateUserRoleCommand, Result<UserRole>>
{
    public Task<Result<UserRole>> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new UserRole
        {
            RoleId = Guid.NewGuid(),
            RoleName = request.RoleName,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
        };
        return repository.CreateAsync(role, cancellationToken);
    }
}

public sealed class UpdateUserRoleCommandHandler(IUserRoleRepository repository)
    : IRequestHandler<UpdateUserRoleCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new UserRole
        {
            RoleId = request.RoleId,
            RoleName = request.RoleName,
            Description = request.Description,
        };
        return repository.UpdateAsync(role, cancellationToken);
    }
}

public sealed class DeleteUserRoleCommandHandler(IUserRoleRepository repository)
    : IRequestHandler<DeleteUserRoleCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
        => repository.DeleteAsync(request.RoleId, cancellationToken);
}

public sealed class GetUserRoleByIdQueryHandler(IUserRoleRepository repository)
    : IRequestHandler<GetUserRoleByIdQuery, Result<UserRole?>>
{
    public Task<Result<UserRole?>> Handle(GetUserRoleByIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdAsync(request.RoleId, cancellationToken);
}

public sealed class GetUserRolesPagedQueryHandler(IUserRoleRepository repository)
    : IRequestHandler<GetUserRolesPagedQuery, Result<PagedResult<UserRole>>>
{
    public Task<Result<PagedResult<UserRole>>> Handle(GetUserRolesPagedQuery request, CancellationToken cancellationToken)
        => repository.GetPagedAsync(request.Request, cancellationToken);
}

public sealed class GetAllUserRolesQueryHandler(IUserRoleRepository repository)
    : IRequestHandler<GetAllUserRolesQuery, Result<List<UserRole>>>
{
    public Task<Result<List<UserRole>>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        => repository.GetAllAsync(cancellationToken);
}



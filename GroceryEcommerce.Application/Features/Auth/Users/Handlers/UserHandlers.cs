using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Users.Commands;
using GroceryEcommerce.Application.Features.Auth.Users.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Users.Handlers;

public sealed class GetUserByIdQueryHandler(IUserRepository repository, ICurrentUserService currentUserService)
    : IRequestHandler<GetUserByIdQuery, Result<User?>>
{
    public Task<Result<User?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdAsync(currentUserService.GetCurrentUserId() ?? request.UserId ?? Guid.Empty, cancellationToken);
}

public sealed class GetUserByEmailQueryHandler(IUserRepository repository)
    : IRequestHandler<GetUserByEmailQuery, Result<User?>>
{
    public Task<Result<User?>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        => repository.GetByEmailAsync(request.Email, cancellationToken);
}

public sealed class GetUserByUsernameQueryHandler(IUserRepository repository)
    : IRequestHandler<GetUserByUsernameQuery, Result<User?>>
{
    public Task<Result<User?>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        => repository.GetByUsernameAsync(request.Username, cancellationToken);
}

public sealed class CreateUserCommandHandler(IUserRepository repository)
    : IRequestHandler<CreateUserCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = request.PasswordHash,
            Status = 1,
            EmailVerified = false,
            PhoneVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        return repository.AddAsync(user, cancellationToken);
    }
}

public sealed class UpdateUserCommandHandler(IUserRepository repository, IPasswordHashService passwordHashService)
    : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.UserId, cancellationToken);
        if (!existing.IsSuccess || existing.Data is null)
            return Result<bool>.Failure("User not found");

        var user = existing.Data;
        user.Email = request.Email ?? user.Email;
        user.Username = request.Username ?? user.Username;
        
        // Hash password if provided (not null or empty)
        if (!string.IsNullOrWhiteSpace(request.PasswordHash))
        {
            user.PasswordHash = passwordHashService.HashPassword(request.PasswordHash);
        }
        
        if (request.FirstName is not null) user.FirstName = request.FirstName;
        if (request.LastName is not null) user.LastName = request.LastName;
        if (request.PhoneNumber is not null) user.PhoneNumber = request.PhoneNumber;
        if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth;
        user.UpdatedAt = DateTime.UtcNow;

        return await repository.UpdateAsync(user, cancellationToken);
    }
}

public sealed class DeleteUserCommandHandler(IUserRepository repository)
    : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.UserId, cancellationToken);
        if (!existing.IsSuccess || existing.Data is null)
            return Result<bool>.Failure("User not found");

        return await repository.DeleteAsync(existing.Data, cancellationToken);
    }
}



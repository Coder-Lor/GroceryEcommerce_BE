using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserAddresses.Commands;
using GroceryEcommerce.Application.Features.Auth.UserAddresses.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserAddresses.Handlers;

public sealed class CreateUserAddressCommandHandler(IUserAddressRepository repository)
    : IRequestHandler<CreateUserAddressCommand, Result<UserAddress>>
{
    public Task<Result<UserAddress>> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = new UserAddress
        {
            AddressId = Guid.NewGuid(),
            UserId = request.UserId,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.PostalCode,
            AddressType = request.AddressType,
            IsDefault = request.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
        return repository.CreateAsync(entity, cancellationToken);
    }
}

public sealed class UpdateUserAddressCommandHandler(IUserAddressRepository repository)
    : IRequestHandler<UpdateUserAddressCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = new UserAddress
        {
            AddressId = request.AddressId,
            UserId = request.UserId,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.PostalCode,
            AddressType = request.AddressType,
            IsDefault = request.IsDefault,
            UpdatedAt = DateTime.UtcNow
        };
        return repository.UpdateAsync(entity, cancellationToken);
    }
}

public sealed class DeleteUserAddressCommandHandler(IUserAddressRepository repository)
    : IRequestHandler<DeleteUserAddressCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
        => repository.DeleteAsync(request.AddressId, cancellationToken);
}

public sealed class SetDefaultUserAddressCommandHandler(IUserAddressRepository repository)
    : IRequestHandler<SetDefaultUserAddressCommand, Result<bool>>
{
    public Task<Result<bool>> Handle(SetDefaultUserAddressCommand request, CancellationToken cancellationToken)
        => repository.SetDefaultAddressAsync(request.UserId, request.AddressId, cancellationToken);
}

public sealed class GetUserAddressesByUserPagedQueryHandler(IUserAddressRepository repository)
    : IRequestHandler<GetUserAddressesByUserPagedQuery, Result<PagedResult<UserAddress>>>
{
    public Task<Result<PagedResult<UserAddress>>> Handle(GetUserAddressesByUserPagedQuery request, CancellationToken cancellationToken)
        => repository.GetByUserIdAsync(request.Request, request.UserId, cancellationToken);
}

public sealed class GetDefaultUserAddressQueryHandler(IUserAddressRepository repository)
    : IRequestHandler<GetDefaultUserAddressQuery, Result<UserAddress?>>
{
    public Task<Result<UserAddress?>> Handle(GetDefaultUserAddressQuery request, CancellationToken cancellationToken)
        => repository.GetDefaultAddressByUserIdAsync(request.UserId, cancellationToken);
}



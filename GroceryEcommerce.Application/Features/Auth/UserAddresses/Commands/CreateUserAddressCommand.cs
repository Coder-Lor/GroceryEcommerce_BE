using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserAddresses.Commands;

public sealed record CreateUserAddressCommand : IRequest<Result<bool>>
{
    public required Guid UserId { get; init; }
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string Country { get; init; }
    public required string PostalCode { get; init; }
    public short AddressType { get; init; } = 0;
    public bool IsDefault { get; init; } = false;
}



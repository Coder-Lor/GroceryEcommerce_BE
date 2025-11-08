using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Users.Commands;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result<bool>>;



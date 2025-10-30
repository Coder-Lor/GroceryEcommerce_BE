using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Users.Queries;

public sealed record GetUserByUsernameQuery(string Username) : IRequest<Result<User?>>;



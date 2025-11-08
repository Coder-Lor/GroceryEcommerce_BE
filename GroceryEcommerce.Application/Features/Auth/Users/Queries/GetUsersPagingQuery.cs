using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.Users.Queries;

public sealed record GetUsersPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<UserDto>>>;

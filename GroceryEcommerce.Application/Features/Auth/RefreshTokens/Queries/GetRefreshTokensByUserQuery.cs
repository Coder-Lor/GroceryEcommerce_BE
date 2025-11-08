using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.RefreshTokens.Queries;

public sealed record GetRefreshTokensByUserQuery(Guid UserId) : IRequest<Result<List<RefreshToken>>>;



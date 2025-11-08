using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserSessions.Commands;

public sealed record RevokeUserSessionCommand(Guid SessionId) : IRequest<Result<bool>>;



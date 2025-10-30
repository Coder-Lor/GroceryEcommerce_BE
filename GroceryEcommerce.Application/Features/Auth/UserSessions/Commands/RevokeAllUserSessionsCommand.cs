using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Auth.UserSessions.Commands;

public sealed record RevokeAllUserSessionsCommand(Guid UserId) : IRequest<Result<bool>>;



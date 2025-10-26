using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogCountByUserQuery(
    Guid UserId
) : IRequest<Result<int>>;

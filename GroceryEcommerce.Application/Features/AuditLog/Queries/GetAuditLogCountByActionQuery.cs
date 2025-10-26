using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogCountByActionQuery(
    string Action
) : IRequest<Result<int>>;

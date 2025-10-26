using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogsByEntityQuery(
    PagedRequest Request,
    string Entity,
    Guid? EntityId = null
) : IRequest<Result<PagedResult<AuditLogDto>>>;

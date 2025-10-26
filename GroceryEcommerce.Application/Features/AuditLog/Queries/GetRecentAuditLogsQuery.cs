using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetRecentAuditLogsQuery(
    PagedRequest Request,
    int Count = 100
) : IRequest<Result<PagedResult<AuditLogDto>>>;

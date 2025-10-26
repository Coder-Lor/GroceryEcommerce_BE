using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogsByUserAndDateRangeQuery(
    PagedRequest Request,
    Guid UserId,
    DateTime FromDate,
    DateTime ToDate
) : IRequest<Result<PagedResult<AuditLogDto>>>;

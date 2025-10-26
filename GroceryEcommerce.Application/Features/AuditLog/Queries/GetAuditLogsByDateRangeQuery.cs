using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogsByDateRangeQuery(
    PagedRequest Request,
    DateTime FromDate,
    DateTime ToDate
) : IRequest<Result<PagedResult<AuditLogDto>>>;

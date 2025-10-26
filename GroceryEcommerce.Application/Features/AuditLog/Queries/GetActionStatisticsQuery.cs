using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetActionStatisticsQuery(
    DateTime FromDate,
    DateTime ToDate
) : IRequest<Result<Dictionary<string, int>>>;

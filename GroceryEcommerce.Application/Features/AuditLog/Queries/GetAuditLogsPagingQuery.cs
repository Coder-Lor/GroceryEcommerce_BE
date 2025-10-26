using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogsPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<AuditLogDto>>>;

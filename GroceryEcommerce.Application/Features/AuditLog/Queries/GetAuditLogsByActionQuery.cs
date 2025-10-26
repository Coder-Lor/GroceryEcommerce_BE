using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogsByActionQuery(
    PagedRequest Request,
    string Action
) : IRequest<Result<PagedResult<AuditLogDto>>>;

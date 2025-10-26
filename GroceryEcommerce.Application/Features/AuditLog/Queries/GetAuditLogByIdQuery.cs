using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Queries;

public record GetAuditLogByIdQuery(
    Guid AuditId
) : IRequest<Result<AuditLogDto?>>;

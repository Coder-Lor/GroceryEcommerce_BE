using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Commands;

public record DeleteAuditLogCommand(
    Guid AuditId
) : IRequest<Result<bool>>;

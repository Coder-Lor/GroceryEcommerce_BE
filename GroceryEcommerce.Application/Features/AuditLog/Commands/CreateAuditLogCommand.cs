using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using MediatR;

namespace GroceryEcommerce.Application.Features.AuditLog.Commands;

public record CreateAuditLogCommand(
    Guid? UserId,
    string Action,
    string? Entity = null,
    Guid? EntityId = null,
    string? Detail = null
) : IRequest<Result<AuditLogDto>>;

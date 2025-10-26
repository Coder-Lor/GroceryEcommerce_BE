using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class DeleteAuditLogCommandHandler(
    IAuditLogRepository repository,
    ILogger<DeleteAuditLogCommandHandler> logger
) : IRequestHandler<DeleteAuditLogCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteAuditLogCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling DeleteAuditLogCommand for AuditId: {AuditId}", request.AuditId);

        var result = await repository.DeleteAsync(request.AuditId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to delete audit log: {AuditId}", request.AuditId);
            return Result<bool>.Failure(result.ErrorMessage);
        }

        logger.LogInformation("Audit log deleted successfully: {AuditId}", request.AuditId);
        return Result<bool>.Success(true);
    }
}

using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogCountByActionHandler(
    IAuditLogRepository repository,
    ILogger<GetAuditLogCountByActionHandler> logger
) : IRequestHandler<GetAuditLogCountByActionQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetAuditLogCountByActionQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogCountByActionQuery for Action: {Action}", request.Action);

        var result = await repository.GetLogCountByActionAsync(request.Action, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to count audit logs for Action: {Action}", request.Action);
            return Result<int>.Failure(result.ErrorMessage);
        }

        return Result<int>.Success(result.Data);
    }
}

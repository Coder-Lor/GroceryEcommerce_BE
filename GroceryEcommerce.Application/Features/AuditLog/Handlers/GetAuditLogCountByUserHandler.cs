using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogCountByUserHandler(
    IAuditLogRepository repository,
    ILogger<GetAuditLogCountByUserHandler> logger
) : IRequestHandler<GetAuditLogCountByUserQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetAuditLogCountByUserQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogCountByUserQuery for UserId: {UserId}", request.UserId);

        var result = await repository.GetLogCountByUserAsync(request.UserId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to count audit logs for UserId: {UserId}", request.UserId);
            return Result<int>.Failure(result.ErrorMessage);
        }

        return Result<int>.Success(result.Data);
    }
}

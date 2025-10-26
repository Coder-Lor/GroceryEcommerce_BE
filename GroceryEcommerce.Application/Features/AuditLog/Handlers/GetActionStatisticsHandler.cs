using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetActionStatisticsHandler(
    IAuditLogRepository repository,
    ILogger<GetActionStatisticsHandler> logger
) : IRequestHandler<GetActionStatisticsQuery, Result<Dictionary<string, int>>>
{
    public async Task<Result<Dictionary<string, int>>> Handle(GetActionStatisticsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetActionStatisticsQuery from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var result = await repository.GetActionStatisticsAsync(request.FromDate, request.ToDate, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to retrieve action statistics");
            return Result<Dictionary<string, int>>.Failure(result.ErrorMessage);
        }

        return Result<Dictionary<string, int>>.Success(result.Data);
    }
}

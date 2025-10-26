using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetRecentAuditLogsHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetRecentAuditLogsHandler> logger
) : IRequestHandler<GetRecentAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetRecentAuditLogsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetRecentAuditLogsQuery with count: {Count}", request.Count);

        var result = await repository.GetRecentLogsAsync(request.Request, request.Count, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Failed to retrieve recent audit logs");
            return Result<PagedResult<AuditLogDto>>.Failure(result.ErrorMessage);
        }

        var pagedDto = mapper.Map<PagedResult<AuditLogDto>>(result.Data);
        return Result<PagedResult<AuditLogDto>>.Success(pagedDto);
    }
}

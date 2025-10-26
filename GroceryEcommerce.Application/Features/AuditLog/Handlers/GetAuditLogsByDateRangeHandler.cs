using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogsByDateRangeHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetAuditLogsByDateRangeHandler> logger
) : IRequestHandler<GetAuditLogsByDateRangeQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsByDateRangeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogsByDateRangeQuery from {FromDate} to {ToDate}", 
            request.FromDate, request.ToDate);

        var result = await repository.GetByDateRangeAsync(request.Request, request.FromDate, request.ToDate, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Failed to retrieve audit logs for date range");
            return Result<PagedResult<AuditLogDto>>.Failure(result.ErrorMessage);
        }

        var pagedDto = mapper.Map<PagedResult<AuditLogDto>>(result.Data);
        return Result<PagedResult<AuditLogDto>>.Success(pagedDto);
    }
}

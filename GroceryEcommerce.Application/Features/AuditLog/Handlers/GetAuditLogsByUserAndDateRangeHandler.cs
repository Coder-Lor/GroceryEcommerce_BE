using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogsByUserAndDateRangeHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetAuditLogsByUserAndDateRangeHandler> logger
) : IRequestHandler<GetAuditLogsByUserAndDateRangeQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsByUserAndDateRangeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogsByUserAndDateRangeQuery for UserId: {UserId}, from {FromDate} to {ToDate}", 
            request.UserId, request.FromDate, request.ToDate);

        var result = await repository.GetByUserAndDateRangeAsync(request.Request, request.UserId, request.FromDate, request.ToDate, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Failed to retrieve audit logs for UserId: {UserId} and date range", request.UserId);
            return Result<PagedResult<AuditLogDto>>.Failure(result.ErrorMessage);
        }

        var pagedDto = mapper.Map<PagedResult<AuditLogDto>>(result.Data);
        return Result<PagedResult<AuditLogDto>>.Success(pagedDto);
    }
}

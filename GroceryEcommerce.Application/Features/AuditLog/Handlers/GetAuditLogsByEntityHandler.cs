using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogsByEntityHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetAuditLogsByEntityHandler> logger
) : IRequestHandler<GetAuditLogsByEntityQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsByEntityQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogsByEntityQuery for Entity: {Entity}, EntityId: {EntityId}", 
            request.Entity, request.EntityId);

        var entityId = request.EntityId ?? Guid.Empty;
        var result = await repository.GetByEntityAsync(request.Request, request.Entity, entityId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Failed to retrieve audit logs for Entity: {Entity}", request.Entity);
            return Result<PagedResult<AuditLogDto>>.Failure(result.ErrorMessage);
        }

        var pagedDto = mapper.Map<PagedResult<AuditLogDto>>(result.Data);
        return Result<PagedResult<AuditLogDto>>.Success(pagedDto);
    }
}

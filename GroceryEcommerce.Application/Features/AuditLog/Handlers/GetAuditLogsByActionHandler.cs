using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogsByActionHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetAuditLogsByActionHandler> logger
) : IRequestHandler<GetAuditLogsByActionQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsByActionQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogsByActionQuery for Action: {Action}", request.Action);

        var result = await repository.GetByActionAsync(request.Request, request.Action, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Failed to retrieve audit logs for Action: {Action}", request.Action);
            return Result<PagedResult<AuditLogDto>>.Failure(result.ErrorMessage);
        }

        var pagedDto = mapper.Map<PagedResult<AuditLogDto>>(result.Data);
        return Result<PagedResult<AuditLogDto>>.Success(pagedDto);
    }
}

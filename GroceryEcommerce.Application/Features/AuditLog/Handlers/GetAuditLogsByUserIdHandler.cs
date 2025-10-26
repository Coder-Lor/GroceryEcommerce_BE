using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogsByUserIdHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetAuditLogsByUserIdHandler> logger
) : IRequestHandler<GetAuditLogsByUserIdQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsByUserIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogsByUserIdQuery for UserId: {UserId}", request.UserId);

        var result = await repository.GetByUserIdAsync(request.Request, request.UserId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Failed to retrieve audit logs for UserId: {UserId}", request.UserId);
            return Result<PagedResult<AuditLogDto>>.Failure(result.ErrorMessage);
        }

        var pagedDto = mapper.Map<PagedResult<AuditLogDto>>(result.Data);
        return Result<PagedResult<AuditLogDto>>.Success(pagedDto);
    }
}

using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogsPagingHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetAuditLogsPagingHandler> logger
) : IRequestHandler<GetAuditLogsPagingQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogsPagingQuery with Page: {Page}, PageSize: {PageSize}", 
            request.Request.Page, request.Request.PageSize);

        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Failed to retrieve paged audit logs");
            return Result<PagedResult<AuditLogDto>>.Failure(result.ErrorMessage);
        }

        var pagedDto = mapper.Map<PagedResult<AuditLogDto>>(result.Data);
        return Result<PagedResult<AuditLogDto>>.Success(pagedDto);
    }
}

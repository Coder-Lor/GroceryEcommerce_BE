using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class GetAuditLogByIdHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<GetAuditLogByIdHandler> logger
) : IRequestHandler<GetAuditLogByIdQuery, Result<AuditLogDto?>>
{
    public async Task<Result<AuditLogDto?>> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetAuditLogByIdQuery for AuditId: {AuditId}", request.AuditId);

        var result = await repository.GetByIdAsync(request.AuditId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to retrieve audit log: {AuditId}", request.AuditId);
            return Result<AuditLogDto?>.Failure(result.ErrorMessage);
        }

        var dto = mapper.Map<AuditLogDto>(result.Data);
        return Result<AuditLogDto?>.Success(dto);
    }
}

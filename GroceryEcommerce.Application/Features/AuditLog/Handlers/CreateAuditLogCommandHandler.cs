using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.AuditLog.Handlers;

public class CreateAuditLogCommandHandler(
    IAuditLogRepository repository,
    IMapper mapper,
    ILogger<CreateAuditLogCommandHandler> logger
) : IRequestHandler<CreateAuditLogCommand, Result<AuditLogDto>>
{
    public async Task<Result<AuditLogDto>> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling CreateAuditLogCommand for Action: {Action}", request.Action);

        var auditLog = new Domain.Entities.Auth.AuditLog
        {
            UserId = request.UserId,
            Action = request.Action,
            Entity = request.Entity,
            EntityId = request.EntityId,
            Detail = request.Detail
        };

        var result = await repository.CreateAsync(auditLog, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogError("Failed to create audit log");
            return Result<AuditLogDto>.Failure(result.ErrorMessage);
        }

        var dto = mapper.Map<AuditLogDto>(result.Data);
        logger.LogInformation("Audit log created successfully: {AuditId}", dto.AuditId);
        return Result<AuditLogDto>.Success(dto);
    }
}

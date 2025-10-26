using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.AuditLog.Commands;
using GroceryEcommerce.Application.Features.AuditLog.Queries;
using GroceryEcommerce.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController(IMediator mediator) : ControllerBase
{
    [HttpGet("{auditId:guid}")]
    public async Task<ActionResult<Result<AuditLogDto?>>> GetById(Guid auditId)
    {
        var query = new GetAuditLogByIdQuery(auditId);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetPaged([FromQuery] PagedRequest request)
    {
        var query = new GetAuditLogsPagingQuery(request);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetByUserId(
        Guid userId,
        [FromQuery] PagedRequest request)
    {
        var query = new GetAuditLogsByUserIdQuery(request, userId);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("entity/{entity}")]
    public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetByEntity(
        string entity,
        [FromQuery] Guid? entityId,
        [FromQuery] PagedRequest request)
    {
        var query = new GetAuditLogsByEntityQuery(request, entity, entityId);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("action/{action}")]
    public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetByAction(
        string action,
        [FromQuery] PagedRequest request)
    {
        var query = new GetAuditLogsByActionQuery(request, action);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("daterange")]
    public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetByDateRange(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] PagedRequest request)
    {
        var query = new GetAuditLogsByDateRangeQuery(request, fromDate, toDate);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("user/{userId:guid}/daterange")]
    public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetByUserAndDateRange(
        Guid userId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] PagedRequest request)
    {
        var query = new GetAuditLogsByUserAndDateRangeQuery(request, userId, fromDate, toDate);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("recent")]
    public async Task<ActionResult<Result<PagedResult<AuditLogDto>>>> GetRecent(
        [FromQuery] int count = 100,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var request = new PagedRequest { Page = page, PageSize = pageSize };
        var query = new GetRecentAuditLogsQuery(request, count);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("count/user/{userId:guid}")]
    public async Task<ActionResult<Result<int>>> GetCountByUser(Guid userId)
    {
        var query = new GetAuditLogCountByUserQuery(userId);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("count/action/{action}")]
    public async Task<ActionResult<Result<int>>> GetCountByAction(string action)
    {
        var query = new GetAuditLogCountByActionQuery(action);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<Result<Dictionary<string, int>>>> GetStatistics(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var query = new GetActionStatisticsQuery(fromDate, toDate);
        var result = await mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<AuditLogDto>>> Create([FromBody] CreateAuditLogCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{auditId:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid auditId)
    {
        var command = new DeleteAuditLogCommand(auditId);
        var result = await mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}

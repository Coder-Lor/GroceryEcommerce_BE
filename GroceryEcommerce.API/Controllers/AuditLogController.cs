using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Domain.Entities.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController(IAuditLogRepository repository) : ControllerBase
{
    [HttpPost("paged")]
    public async Task<ActionResult<Result<PagedResult<AuditLog>>>> GetPaged([FromBody] PagedRequest request)
    {
        var result = await repository.GetPagedAsync(request);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<AuditLog?>>> GetById(Guid id)
    {
        var result = await repository.GetByIdAsync(id);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<AuditLog>>> Create([FromBody] AuditLog auditLog)
    {
        var result = await repository.CreateAsync(auditLog);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id)
    {
        var result = await repository.DeleteAsync(id);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("by-user/{userId:guid}")]
    public async Task<ActionResult<Result<PagedResult<AuditLog>>>> GetByUser([FromBody] PagedRequest request, Guid userId)
    {
        var result = await repository.GetByUserIdAsync(request, userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("by-entity")]
    public async Task<ActionResult<Result<PagedResult<AuditLog>>>> GetByEntity([FromBody] PagedRequest request, [FromQuery] string entity, [FromQuery] Guid entityId)
    {
        var result = await repository.GetByEntityAsync(request, entity, entityId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("by-action")]
    public async Task<ActionResult<Result<PagedResult<AuditLog>>>> GetByAction([FromBody] PagedRequest request, [FromQuery] string action)
    {
        var result = await repository.GetByActionAsync(request, action);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("by-date-range")]
    public async Task<ActionResult<Result<PagedResult<AuditLog>>>> GetByDateRange([FromBody] PagedRequest request, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await repository.GetByDateRangeAsync(request, from, to);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("recent")]
    public async Task<ActionResult<Result<PagedResult<AuditLog>>>> GetRecent([FromBody] PagedRequest request, [FromQuery] int count = 100)
    {
        var result = await repository.GetRecentLogsAsync(request, count);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("count/by-user/{userId:guid}")]
    public async Task<ActionResult<Result<int>>> GetLogCountByUser(Guid userId)
    {
        var result = await repository.GetLogCountByUserAsync(userId);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("count/by-action")]
    public async Task<ActionResult<Result<int>>> GetLogCountByAction([FromQuery] string action)
    {
        var result = await repository.GetLogCountByActionAsync(action);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<Result<Dictionary<string, int>>>> GetActionStatistics([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await repository.GetActionStatisticsAsync(from, to);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

}

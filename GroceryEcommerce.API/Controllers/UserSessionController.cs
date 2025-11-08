using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserSessions.Commands;
using GroceryEcommerce.Application.Features.Auth.UserSessions.Queries;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserSessionController(IMediator mediator) : ControllerBase
{
    [HttpPost("users/{userId:guid}/paged")]
    public async Task<ActionResult<Result<PagedResult<UserSession>>>> GetByUser([FromBody] PagedRequest request, Guid userId)
    {
        var result = await mediator.Send(new GetUserSessionsByUserPagedQuery(request, userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/active-paged")]
    public async Task<ActionResult<Result<PagedResult<UserSession>>>> GetActiveByUser([FromBody] PagedRequest request, Guid userId)
    {
        var result = await mediator.Send(new GetActiveUserSessionsByUserPagedQuery(request, userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("revoke/{sessionId:guid}")]
    public async Task<ActionResult<Result<bool>>> Revoke(Guid sessionId)
    {
        var result = await mediator.Send(new RevokeUserSessionCommand(sessionId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/revoke-all")]
    public async Task<ActionResult<Result<bool>>> RevokeAll(Guid userId)
    {
        var result = await mediator.Send(new RevokeAllUserSessionsCommand(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}



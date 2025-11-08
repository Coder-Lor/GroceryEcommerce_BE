using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.RefreshTokens.Commands;
using GroceryEcommerce.Application.Features.Auth.RefreshTokens.Queries;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefreshTokenController(IMediator mediator) : ControllerBase
{
    [HttpGet("users/{userId:guid}")]
    public async Task<ActionResult<Result<List<RefreshToken>>>> GetByUser(Guid userId)
    {
        var result = await mediator.Send(new GetRefreshTokensByUserQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("revoke/{tokenId:guid}")]
    public async Task<ActionResult<Result<bool>>> Revoke(Guid tokenId)
    {
        var result = await mediator.Send(new RevokeRefreshTokenCommand(tokenId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/revoke-all")]
    public async Task<ActionResult<Result<bool>>> RevokeAll(Guid userId)
    {
        var result = await mediator.Send(new RevokeAllUserTokensCommand(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("cleanup-expired")]
    public async Task<ActionResult<Result<bool>>> CleanupExpired()
    {
        var result = await mediator.Send(new CleanupExpiredRefreshTokensCommand());
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}



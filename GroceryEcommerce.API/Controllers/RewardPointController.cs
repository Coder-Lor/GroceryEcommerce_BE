using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RewardPointController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<RewardPointDto>>> Create([FromBody] CreateRewardPointCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<RewardPointDto>>> Update([FromBody] UpdateRewardPointCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{rewardPointId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid rewardPointId)
    {
        var command = new DeleteRewardPointCommand(rewardPointId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{rewardPointId}")]
    public async Task<ActionResult<Result<RewardPointDto?>>> GetById([FromRoute] Guid rewardPointId)
    {
        var query = new GetRewardPointByIdQuery(rewardPointId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<Result<List<RewardPointDto>>>> GetByUserId([FromRoute] Guid userId)
    {
        var query = new GetRewardPointsByUserIdQuery(userId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<RewardPointDto>>>> GetPaging([FromQuery] PagedRequest request)
    {
        var query = new GetRewardPointsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}


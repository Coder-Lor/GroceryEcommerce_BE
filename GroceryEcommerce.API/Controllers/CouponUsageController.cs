using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Queries;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CouponUsageController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<CouponUsageDto>>> Create([FromBody] CreateCouponUsageCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<CouponUsageDto>>> Update([FromBody] UpdateCouponUsageCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{usageId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid usageId)
    {
        var command = new DeleteCouponUsageCommand(usageId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{usageId}")]
    public async Task<ActionResult<Result<CouponUsageDto?>>> GetById([FromRoute] Guid usageId)
    {
        var query = new GetCouponUsageByIdQuery(usageId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<CouponUsageDto>>>> GetPaging([FromQuery] PagedRequest request)
    {
        var query = new GetCouponUsagesPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("coupon/{couponId}")]
    public async Task<ActionResult<Result<PagedResult<CouponUsageDto>>>> GetByCouponId(
        [FromRoute] Guid couponId,
        [FromQuery] PagedRequest request)
    {
        var query = new GetCouponUsagesByCouponIdQuery(couponId, request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<Result<PagedResult<CouponUsageDto>>>> GetByUserId(
        [FromRoute] Guid userId,
        [FromQuery] PagedRequest request)
    {
        var query = new GetCouponUsagesByUserIdQuery(userId, request);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}


using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Commands;
using GroceryEcommerce.Application.Features.Catalog.Shop.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<ShopDto>>>> GetShopsPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetShopsPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<Result<PagedResult<ShopDto>>>> GetActiveShopsPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetActiveShopsPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("{shopId:guid}")]
    public async Task<ActionResult<Result<GetShopByIdResponse>>> GetById([FromRoute] Guid shopId)
    {
        var result = await mediator.Send(new GetShopByIdQuery(shopId));
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Result<GetShopBySlugResponse>>> GetBySlug([FromRoute] string slug)
    {
        var result = await mediator.Send(new GetShopBySlugQuery(slug));
        return Ok(result);
    }

    [HttpGet("owner/{ownerUserId:guid}")]
    public async Task<ActionResult<Result<PagedResult<ShopDto>>>> GetByOwner(
        [FromRoute] Guid ownerUserId,
        [FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetShopsByOwnerQuery(ownerUserId, request));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<CreateShopResponse>>> Create([FromBody] CreateShopCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{shopId:guid}")]
    public async Task<ActionResult<Result<UpdateShopResponse>>> Update(
        [FromRoute] Guid shopId,
        [FromBody] UpdateShopCommand command)
    {
        // đảm bảo id route và body khớp nhau
        if (shopId != command.ShopId)
        {
            return BadRequest(Result<UpdateShopResponse>.Failure("Shop ID in route and body do not match"));
        }

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("status/{shopId:guid}")]
    public async Task<ActionResult<Result<bool>>> UpdateStatus(
        [FromRoute] Guid shopId,
        [FromBody] UpdateShopStatusCommand command)
    {
        if (shopId != command.ShopId)
        {
            return BadRequest(Result<bool>.Failure("Shop ID in route and body do not match"));
        }

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{shopId:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid shopId)
    {
        var result = await mediator.Send(new DeleteShopCommand(shopId));
        return Ok(result);
    }
}



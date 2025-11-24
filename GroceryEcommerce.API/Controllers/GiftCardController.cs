using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Commands;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Queries;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GiftCardController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<GiftCardDto>>> Create([FromBody] CreateGiftCardCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<GiftCardDto>>> Update([FromBody] UpdateGiftCardCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{giftCardId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid giftCardId)
    {
        var command = new DeleteGiftCardCommand(giftCardId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{giftCardId}")]
    public async Task<ActionResult<Result<GiftCardDto?>>> GetById([FromRoute] Guid giftCardId)
    {
        var query = new GetGiftCardByIdQuery(giftCardId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("code/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<GiftCardDto?>>> GetByCode([FromRoute] string code)
    {
        var query = new GetGiftCardByCodeQuery(code);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<GiftCardDto>>>> GetPaging([FromQuery] PagedRequest request)
    {
        var query = new GetGiftCardsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}


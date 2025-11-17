using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Commands;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipmentCarrierController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<ShipmentCarrierDto>>>> GetShipmentCarriersPaging([FromQuery] PagedRequest request)
    {
        var query = new GetShipmentCarriersPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<ShipmentCarrierDto>>>> GetAll()
    {
        var query = new GetAllShipmentCarriersQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<ShipmentCarrierDto>>>> GetActive()
    {
        var query = new GetActiveShipmentCarriersQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{carrierId}")]
    public async Task<ActionResult<Result<ShipmentCarrierDto>>> GetById([FromRoute] Guid carrierId)
    {
        var query = new GetShipmentCarrierByIdQuery(carrierId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<Result<ShipmentCarrierDto>>> GetByName([FromRoute] string name)
    {
        var query = new GetShipmentCarrierByNameQuery(name);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<ShipmentCarrierDto>>> Create([FromBody] CreateShipmentCarrierRequest request)
    {
        var command = new CreateShipmentCarrierCommand(request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update/{carrierId}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] Guid carrierId,
        [FromBody] UpdateShipmentCarrierRequest request)
    {
        var command = new UpdateShipmentCarrierCommand(carrierId, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{carrierId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid carrierId)
    {
        var command = new DeleteShipmentCarrierCommand(carrierId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}


using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserAddresses.Commands;
using GroceryEcommerce.Application.Features.Auth.UserAddresses.Queries;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAddressController(IMediator mediator) : ControllerBase
{
    [HttpPost("users/{userId:guid}/paged")]
    public async Task<ActionResult<Result<PagedResult<UserAddress>>>> GetByUser([FromBody] PagedRequest request, Guid userId)
    {
        var result = await mediator.Send(new GetUserAddressesByUserPagedQuery(request, userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("users/{userId:guid}/default")]
    public async Task<ActionResult<Result<UserAddress?>>> GetDefault(Guid userId)
    {
        var result = await mediator.Send(new GetDefaultUserAddressQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<UserAddress>>> Create([FromBody] CreateUserAddressCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<bool>>> Update([FromBody] UpdateUserAddressCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/default/{addressId:guid}")]
    public async Task<ActionResult<Result<bool>>> SetDefault(Guid userId, Guid addressId)
    {
        var result = await mediator.Send(new SetDefaultUserAddressCommand(userId, addressId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{addressId:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid addressId)
    {
        var result = await mediator.Send(new DeleteUserAddressCommand(addressId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}



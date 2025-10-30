using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Users.Commands;
using GroceryEcommerce.Application.Features.Auth.Users.Queries;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<Result<User?>>> GetById(Guid userId)
    {
        var result = await mediator.Send(new GetUserByIdQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("by-email")]
    public async Task<ActionResult<Result<User?>>> GetByEmail([FromQuery] string email)
    {
        var result = await mediator.Send(new GetUserByEmailQuery(email));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("by-username")]
    public async Task<ActionResult<Result<User?>>> GetByUsername([FromQuery] string username)
    {
        var result = await mediator.Send(new GetUserByUsernameQuery(username));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<bool>>> Create([FromBody] CreateUserCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<bool>>> Update([FromBody] UpdateUserCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid userId)
    {
        var result = await mediator.Send(new DeleteUserCommand(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}



using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserRoles.Commands;
using GroceryEcommerce.Application.Features.Auth.UserRoles.Queries;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<UserRole>>>> GetPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetUserRolesPagedQuery(request));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{roleId:guid}")]
    public async Task<ActionResult<Result<UserRole?>>> GetById(Guid roleId)
    {
        var result = await mediator.Send(new GetUserRoleByIdQuery(roleId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<ActionResult<Result<List<UserRole>>>> GetAll()
    {
        var result = await mediator.Send(new GetAllUserRolesQuery());
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<UserRole>>> Create([FromBody] CreateUserRoleCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<bool>>> Update([FromBody] UpdateUserRoleCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{roleId:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid roleId)
    {
        var result = await mediator.Send(new DeleteUserRoleCommand(roleId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}



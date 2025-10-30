using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Commands;
using GroceryEcommerce.Application.Features.Auth.UserRoleAssignments.Queries;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRoleAssignmentController(IMediator mediator) : ControllerBase
{
    [HttpPost("assign")]
    public async Task<ActionResult<Result<bool>>> Assign([FromBody] AssignUserRoleCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("remove")]
    public async Task<ActionResult<Result<bool>>> Remove([FromBody] RemoveUserRoleCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("users/{userId:guid}/roles")]
    public async Task<ActionResult<Result<List<string>>>> GetUserRoleNames(Guid userId)
    {
        var result = await mediator.Send(new GetUserRolesOfUserQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/paged")]
    public async Task<ActionResult<Result<PagedResult<UserRoleAssignment>>>> GetAssignmentsByUser([FromBody] PagedRequest request, Guid userId)
    {
        var result = await mediator.Send(new GetUserRoleAssignmentsByUserPagedQuery(request, userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}



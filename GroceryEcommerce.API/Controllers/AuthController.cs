using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Authentication.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Result<RegisterResponse>>> RegisterAccount ([FromBody]RegisterCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result); 
        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<Result<LoginResponse>>> Login ([FromBody] LoginCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result); 
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<Result<bool>>> ForgotPassword([FromBody] ForgotPasswordCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result); 
        return Ok(result);
    }
    
    [HttpPost("reset-password")]
    public async Task<ActionResult<Result<bool>>> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result); 
        return Ok(result);   
    }
    
    
}
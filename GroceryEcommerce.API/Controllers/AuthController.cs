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
        if (result.Data is not null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
        }
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
    
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Response.Cookies to add cookie
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

    }

}
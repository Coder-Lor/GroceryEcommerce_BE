using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<Result<RegisterResponse>>> RegisterAccount ([FromBody]RegisterCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result);
        if (result.Data is not null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
            SetAccessTokenCookie(result.Data.Token);
        }
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<Result<LoginResponse>>> Login ([FromBody] LoginCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result);
        if (result.Data is not null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
            SetAccessTokenCookie(result.Data.Token); // Đúng property access token cho LoginResponse
        }
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<ActionResult<Result<bool>>> ForgotPassword([FromBody] ForgotPasswordCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result); 
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult<Result<bool>>> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result); 
        return Ok(result);   
    }
    
    [HttpPost("logout")]
    public async Task<ActionResult<Result<bool>>> Logout([FromBody] LogoutCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result);
        
        // Clear refresh token cookie
        Response.Cookies.Delete("refreshToken");
        Response.Cookies.Delete("accessToken"); 
        
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<Result<RefreshTokenResponse>>> RefreshToken([FromBody] RefreshTokenCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(result);
        
        if (result.Data is not null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
            SetAccessTokenCookie(result.Data.AccessToken); 
        }
        
        return Ok(result);
    }
    

    private void SetAccessTokenCookie(string accessToken)
    {
        Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(10)
        });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

}
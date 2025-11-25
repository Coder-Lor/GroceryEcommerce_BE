using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<Result<AuthPublicResponse>>> RegisterAccount([FromBody] RegisterCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(Result<AuthPublicResponse>.Failure(result.ErrorMessage ?? "Registration failed", result.ErrorCode));

        if (result.Data is not null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
            SetAccessTokenCookie(result.Data.Token);
            var sanitized = MapToPublic(result.Data);
            return Ok(Result<AuthPublicResponse>.Success(sanitized));
        }

        return Ok(Result<AuthPublicResponse>.Failure("Registration failed", result.ErrorCode));
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<Result<AuthPublicResponse>>> Login([FromBody] LoginCommand request)
    {
        var result = await mediator.Send(request);
        if (!result.IsSuccess)
            return BadRequest(Result<AuthPublicResponse>.Failure(result.ErrorMessage ?? "Login failed", result.ErrorCode));

        if (result.Data is not null)
        {
            SetRefreshTokenCookie(result.Data.RefreshToken);
            SetAccessTokenCookie(result.Data.Token);
            var sanitized = MapToPublic(result.Data);
            return Ok(Result<AuthPublicResponse>.Success(sanitized));
        }

        return Ok(Result<AuthPublicResponse>.Failure("Login failed", result.ErrorCode));
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
    public async Task<ActionResult<Result<bool>>> Logout([FromBody] LogoutCommand? request)
    {
        // Prefer cookie, fallback body for old clients
        var refreshToken = Request.Cookies["refreshToken"] ?? request?.RefreshToken ?? string.Empty;
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("accessToken");
            return Ok(Result<bool>.Success(true));
        }

        var result = await mediator.Send(new LogoutCommand(refreshToken));
        if (!result.IsSuccess)
            return BadRequest(result);
        
        Response.Cookies.Delete("refreshToken");
        Response.Cookies.Delete("accessToken"); 
        
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<Result<AccessTokenPublicResponse>>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(Result<AccessTokenPublicResponse>.Failure("Refresh Token not provided in cookies."));

        var command = new RefreshTokenCommand(refreshToken);
        var result = await mediator.Send(command);

        if (!result.IsSuccess || result.Data is null)
        {
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("accessToken");
            return Unauthorized(Result<AccessTokenPublicResponse>.Failure(result.ErrorMessage ?? "Refresh failed", result.ErrorCode));
        }
        
        SetRefreshTokenCookie(result.Data.RefreshToken);
        SetAccessTokenCookie(result.Data.AccessToken); 
        var sanitized = new AccessTokenPublicResponse
        {
            AccessToken = result.Data.AccessToken,
            ExpiresAt = result.Data.ExpiresAt
        };
        return Ok(Result<AccessTokenPublicResponse>.Success(sanitized));
    }
    

    private void SetAccessTokenCookie(string accessToken)
    {
        Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
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

    private static AuthPublicResponse MapToPublic(RegisterResponse data)
    {
        return new AuthPublicResponse
        {
            UserId = data.UserId,
            Username = data.Username,
            Role = data.Role,
            Email = data.Email,
            Token = data.Token,
            ExpiresAt = data.ExpiresAt
        };
    }

    private static AuthPublicResponse MapToPublic(LoginResponse data)
    {
        return new AuthPublicResponse
        {
            UserId = data.UserId,
            Username = data.Username,
            Role = data.Role,
            Email = data.Email,
            Token = data.Token,
            ExpiresAt = data.ExpiresAt
        };
    }
}

public class AuthPublicResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class AccessTokenPublicResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

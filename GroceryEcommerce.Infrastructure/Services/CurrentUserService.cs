using System.Security.Claims;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId != null ? Guid.Parse(userId) : null;
    }

    public string? GetCurrentUserEmail()
    {
        var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        return email;
    }

    public string? GetCurrentUserName()
    {
        var username = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        return username;
    }

    public List<string> GetCurrentUserRoles()
    {
        var roles = _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        return roles ?? new List<string>();
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }

    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}
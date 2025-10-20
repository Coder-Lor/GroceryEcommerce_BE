using System.Security.Claims;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? GetCurrentUserId();
    string? GetCurrentUserEmail();
    string? GetCurrentUserName();
    List<string> GetCurrentUserRoles();
    bool IsAuthenticated();
    ClaimsPrincipal? GetCurrentUser();
}
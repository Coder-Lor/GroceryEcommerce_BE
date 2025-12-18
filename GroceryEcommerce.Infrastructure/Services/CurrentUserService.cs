using System.Security.Claims;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, IShopRepository shopRepository) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly IShopRepository _shopRepository = shopRepository ?? throw new ArgumentNullException(nameof(shopRepository));

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

    public Guid? GetCurrentUserShopId()
    {
        var userId = GetCurrentUserId();
        if (userId is null || userId == Guid.Empty)
        {
            return null;
        }

        // Vì CurrentUserService không async, tạm thời lấy shop đầu tiên theo OwnerUserId
        // Nên chỉ dùng cho các user có 1 shop.
        var shopsTask = _shopRepository.GetByOwnerAsync(userId.Value, new Application.Common.PagedRequest
        {
            Page = 1,
            PageSize = 1
        });

        var shopsResult = shopsTask.GetAwaiter().GetResult();
        if (!shopsResult.IsSuccess || shopsResult.Data is null || shopsResult.Data.Items.Count == 0)
        {
            return null;
        }

        return shopsResult.Data.Items.First().ShopId;
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
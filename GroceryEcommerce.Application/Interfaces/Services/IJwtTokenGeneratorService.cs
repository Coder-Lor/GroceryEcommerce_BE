using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IJwtTokenGeneratorService
{
    Result<TokenResult> GenerateToken(User user);
    Result<TokenResult> RefreshToken(string refreshToken);
}
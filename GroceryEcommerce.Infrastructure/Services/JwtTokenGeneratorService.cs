using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Infrastructure.Services;

public class JwtTokenGeneratorService : IJwtTokenGeneratorService
{
    public Result<TokenResult> GenerateToken(User user)
    {
        throw new NotImplementedException();
    }

    public Result<TokenResult> RefreshToken(string refreshToken)
    {
        throw new NotImplementedException();
    }
}
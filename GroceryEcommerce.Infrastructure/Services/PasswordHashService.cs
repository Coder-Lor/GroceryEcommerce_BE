using GroceryEcommerce.Application.Interfaces.Services;

namespace GroceryEcommerce.Infrastructure.Services;

public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, string hash)
    {
        throw new NotImplementedException();
    }
}
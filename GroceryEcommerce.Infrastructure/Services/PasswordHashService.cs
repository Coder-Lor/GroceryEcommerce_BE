using System.Security.Cryptography;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace GroceryEcommerce.Infrastructure.Services;

public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password)
    {
        // Generate a random salt
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));

        // Combine salt and hash
        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            var parts = hash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var existingHash = parts[1];

            var newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                100000,
                256 / 8));

            return newHash == existingHash;
        }
        catch
        {
            return false;
        }
    }
}
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GroceryEcommerce.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public TokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
        _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey");
        _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
        _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
        _accessTokenExpirationMinutes = int.Parse(configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");
        _refreshTokenExpirationDays = int.Parse(configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
    }

    public async Task<string> GenerateAccessTokenAsync(Guid userId, string email, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return await Task.FromResult(tokenString);
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, string? ipAddress = null)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var refreshToken = Convert.ToBase64String(randomBytes);

        return await Task.FromResult(refreshToken);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string refreshToken, string? ipAddress = null)
    {
        var token = new RefreshToken
        {
            UserId = userId,
            RefreshTokenValue = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            Revoked = false
        };

        var res = await _refreshTokenRepository.CreateAsync(token);
        return res.IsSuccess ? res.Data : null!;
    }

    public async Task<bool> CreateRefreshTokenInDatabaseAsync(Guid userId, string refreshToken, string? ipAddress = null)
    {
        try
        {
            var token = new RefreshToken
            {
                UserId = userId,
                RefreshTokenValue = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                Revoked = false
            };

            var res = await _refreshTokenRepository.CreateAsync(token);
            return res.IsSuccess;
        }
        catch
        {
            return false;
        }
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        try
        {
            var result = await _refreshTokenRepository.GetByTokenAsync(token);
            return result.IsSuccess ? result.Data : null;
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in GetRefreshTokenAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<RefreshToken?> GetActiveRefreshTokenAsync(Guid userId)
    {
        try
        {
            var result = await _refreshTokenRepository.GetByUserIdAsync(userId);
            if (!result.IsSuccess || result.Data == null)
                return null;

            return await Task.FromResult(result.Data.FirstOrDefault(t =>
                !t.Revoked &&
                t.ExpiresAt > DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in GetActiveRefreshTokenAsync: {ex.Message}");
            return null;
        }
    }


    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        try
        {
            var result = await _refreshTokenRepository.GetByTokenAsync(token);

            if (!result.IsSuccess || result.Data == null)
                return false;

            var refreshToken = result.Data;

            if (refreshToken.Revoked)
                return false;

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
                return false;

            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in ValidateRefreshTokenAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ValidateAndCreateRefreshTokenAsync(string token, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        try
        {
            var result = await _refreshTokenRepository.GetByTokenAsync(token);
            if (result.IsSuccess && result.Data != null)
            {
                var refreshToken = result.Data;

                if (refreshToken.UserId != userId)
                    return false;

                if (refreshToken.Revoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
                    return false;

                return true;
            }

            var newToken = new RefreshToken
            {
                UserId = userId,
                RefreshTokenValue = token,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                Revoked = false
            };

            var createResult = await _refreshTokenRepository.CreateAsync(newToken);
            return createResult.IsSuccess;
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in ValidateAndCreateRefreshTokenAsync: {ex.Message}");
            return false;
        }
    }

    public async Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null)
    {
        try
        {
            var result = await _refreshTokenRepository.GetByTokenAsync(token);

            if (!result.IsSuccess || result.Data == null)
                return;

            var refreshToken = result.Data;
            refreshToken.Revoked = true;
            refreshToken.ReplacedByToken = replacedByToken;

            await _refreshTokenRepository.UpdateAsync(refreshToken);
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in RevokeRefreshTokenAsync: {ex.Message}");
        }
    }

    public async Task RevokeAllUserRefreshTokensAsync(Guid userId)
    {
        try
        {
            var result = await _refreshTokenRepository.GetByUserIdAsync(userId);

            if (!result.IsSuccess || result.Data == null)
                return;

            foreach (var token in result.Data)
            {
                if (!token.Revoked)
                {
                    token.Revoked = true;
                    await _refreshTokenRepository.UpdateAsync(token);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in RevokeAllUserRefreshTokensAsync: {ex.Message}");
        }
    }

    public async Task CleanupExpiredTokensAsync()
    {
        // Lấy tất cả tokens của user hoặc implement theo cách khác
        await Task.CompletedTask;
    }

    public async Task<bool> IsTokenRevokedAsync(string token)
    {
        try
        {
            var result = await _refreshTokenRepository.GetByTokenAsync(token);

            if (!result.IsSuccess || result.Data == null)
                return true;

            return await Task.FromResult(result.Data.Revoked);
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in IsTokenRevokedAsync: {ex.Message}");
            return true; // Assume revoked if error occurs
        }
    }
    
    public Task<string> GetUserIdFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = false
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Task.FromResult(userId ?? throw new SecurityTokenException("Invalid token"));
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GroceryEcommerce.Infrastructure.Services;

public class JwtTokenGeneratorService : IJwtTokenGeneratorService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public JwtTokenGeneratorService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey");
        _issuer = configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer");
        _audience = configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience");
        _accessTokenExpirationMinutes = int.Parse(configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "60");
        _refreshTokenExpirationDays = int.Parse(configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
    }

    public Result<TokenResult> GenerateToken(User user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.LastName ?? user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

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

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            var tokenResult = new TokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expires
            };

            return Result<TokenResult>.Success(tokenResult);
        }
        catch (Exception ex)
        {
            return Result<TokenResult>.Failure($"Error generating token: {ex.Message}");
        }
    }

    public Result<TokenResult> RefreshToken(string refreshToken)
    {
        try
        {
            // Bạn cần validate refresh token từ database
            // Đây là implementation cơ bản
            
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
                ValidateLifetime = false // Không validate lifetime vì đang refresh
            };

            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out var validatedToken);
            
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Result<TokenResult>.Failure("Invalid refresh token");
            }

            // Tạo token mới
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId ?? ""),
                new(ClaimTypes.Email, email ?? ""),
                new(ClaimTypes.Name, name ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var newKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(newKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var accessToken = tokenHandler.WriteToken(token);
            var newRefreshToken = GenerateRefreshToken();

            var tokenResult = new TokenResult
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expires
            };

            return Result<TokenResult>.Success(tokenResult);
        }
        catch (Exception ex)
        {
            return Result<TokenResult>.Failure($"Error refreshing token: {ex.Message}");
        }
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

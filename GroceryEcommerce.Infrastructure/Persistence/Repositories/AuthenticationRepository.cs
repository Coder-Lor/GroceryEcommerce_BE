using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.HelperClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories;

public class AuthenticationRepository(
    IDataAccessAdapterFactory adapterFactory,
    IMapper mapper,
    ILogger<AuthenticationRepository> logger
) : IAuthenticationRepository
{
    public Task<Result<User?>> ValidateUserCredentialsAsync(string emailOrUsername, string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var entity = new UserEntity();

            var filterPredicate = new PredicateExpression();
            filterPredicate.Add(UserFields.Email == emailOrUsername);

        }
        catch (Exception ex)
        {
            
        }
        
        throw new NotImplementedException();
    }

    public Task<Result<bool>> CreateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> SaveRefreshTokenAsync(RefreshTokens refreshTokens, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string?>> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateLastLoginAsync(Guid userId, DateTime lastLogin, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RecordFailedLoginAttemptAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetFailedLoginAttemptsAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ResetFailedLoginAttemptsAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
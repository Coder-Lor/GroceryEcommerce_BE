using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Authentication.Commands;

public sealed class LoginCommandHandler(
    IAuthenticationRepository authenticationRepository,
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    ITokenService tokenService,
    ICacheService cacheService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private const int MaxFailedAttempts = 5;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILogger<LoginCommandHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;
    
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for: {EmailOrUsername}", request.EmailOrUsername);
        
        var failedAttempts = await _authenticationRepository.GetFailedLoginAttemptsAsync(request.EmailOrUsername, cancellationToken);
        if (failedAttempts is { IsSuccess: true, Data: > MaxFailedAttempts })
        {
            _logger.LogWarning("Login failed: Too many failed login attempts for {EmailOrUsername}", request.EmailOrUsername);
            return Result<LoginResponse>.Failure("Too many failed login attempts");
        }
        
        var userResult = await _userRepository.GetUserByEmailOrUsernameAsync(request.EmailOrUsername, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data is null)
        {
            await _authenticationRepository.RecordFailedLoginAttemptAsync(request.EmailOrUsername, cancellationToken);
            _logger.LogWarning("Login failed: User not found - {EmailOrUsername}", request.EmailOrUsername);
            return Result<LoginResponse>.Failure("Invalid credentials");
        }

        var user = userResult.Data;
        if (!_passwordHashService.VerifyPassword(request.Password, user.PasswordHash)) 
        {
            await _authenticationRepository.RecordFailedLoginAttemptAsync(request.EmailOrUsername, cancellationToken);
            _logger.LogWarning("Login failed: Invalid credentials - {EmailOrUsername}", request.EmailOrUsername);
            return Result<LoginResponse>.Failure("Invalid credentials");
        }
        
        await _authenticationRepository.ResetFailedLoginAttemptsAsync(request.EmailOrUsername, cancellationToken);
            
        // gen tokens       
        var userRoles = await _authenticationRepository.GetUserRolesAsync(user.UserId, cancellationToken);
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user.UserId, user.Email, new List<string> {"User"});
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.UserId);
        await _authenticationRepository.UpdateLastLoginAsync(user.UserId, DateTime.UtcNow, cancellationToken);

        
        // store refesh token in cache
        var tokenExpity = DateTime.UtcNow.AddDays(7);
        await _authenticationRepository.CreateRefreshTokenAsync(user.UserId, refreshToken, tokenExpity, cancellationToken);     

        _logger.LogInformation("User logged in successfully: {UserId}", user.UserId);

        var response = new LoginResponse
        {
            UserId = user.UserId.ToString(),
            Email = user.Email,
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        return Result<LoginResponse>.Success(response);

    }
}
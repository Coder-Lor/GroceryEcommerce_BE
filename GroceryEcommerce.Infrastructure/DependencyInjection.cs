using AutoMapper;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Infrastructure.Mapping;
using GroceryEcommerce.Infrastructure.Persistence.Repositories;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;
using GroceryEcommerce.Infrastructure.Services;
using GroceryEcommerce.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Repositories (Interfaces only - implementations will be created later)
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });
        
        services.AddScoped<IUserRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            var logger = provider.GetRequiredService<ILogger<UserRepository>>();
            return new UserRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IAuthenticationRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var passwordHashService = provider.GetRequiredService<IPasswordHashService>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<AuthenticationRepository>>();
            return new AuthenticationRepository(adapter, passwordHashService, mapper, logger);
        });
        
        services.AddScoped<IRefreshTokenRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<RefreshTokenRepository>>();
            return new RefreshTokenRepository(adapter, mapper, logger);
        });
        
        services.AddScoped<ICartRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<CartRepository>>();
            var unitOfWorkService = provider.GetRequiredService<IUnitOfWorkService>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new CartRepository(adapter, mapper, unitOfWorkService, cacheService, logger);
        });
        
        // services.AddScoped<ICatalogRepository, CatalogRepository>();
        // services.AddScoped<ISalesRepository, SalesRepository>();
        // services.AddScoped<IInventoryRepository, InventoryRepository>();
        // services.AddScoped<IMarketingRepository, MarketingRepository>();
        // services.AddScoped<IReviewsRepository, ReviewsRepository>();
        // services.AddScoped<ISystemRepository, SystemRepository>();

        // Register Services (Interfaces only - implementations will be created later)
        // services.AddScoped<ICatalogService, CatalogService>();
        // services.AddScoped<ICartService, CartService>();
        // services.AddScoped<ISalesService, SalesService>();
        // services.AddScoped<IInventoryService, InventoryService>();
        // services.AddScoped<IMarketingService, MarketingService>();
        // services.AddScoped<IReviewsService, ReviewsService>();
        // services.AddScoped<ISystemService, SystemService>();

        // Register existing services
        services.AddMemoryCache();
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtTokenGeneratorService, JwtTokenGeneratorService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUnitOfWorkService>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            return new UnitOfWorkService(adapter);
        });
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}

using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Infrastructure.Persistence.Repositories;
using GroceryEcommerce.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryEcommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Cache Service
        services.AddScoped<ICacheService, CacheService>();

        // Register Repositories (Interfaces only - implementations will be created later)
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        // services.AddScoped<ICatalogRepository, CatalogRepository>();
        // services.AddScoped<ICartRepository, CartRepository>();
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
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtTokenGeneratorService, JwtTokenGeneratorService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}

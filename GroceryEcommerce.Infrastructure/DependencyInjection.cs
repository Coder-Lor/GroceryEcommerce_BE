using AutoMapper;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Infrastructure.Mapping;
using GroceryEcommerce.Infrastructure.Persistence.Repositories;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Catalog;
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

        // catalog repositories
        
        services.AddScoped<ICategoryRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<CategoryRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new CategoryRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IBrandRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<BrandRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new BrandRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IProductRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IProductAttributeRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductAttributeRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductAttributeRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IProductAttributeValueRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductAttributeValueRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductAttributeValueRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IProductImageRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductImageRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductImageRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IProductQuestionRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductQuestionRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductQuestionRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IProductTagRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductTagRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductTagRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<IProductTagAssignmentRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductTagAssignmentRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductTagAssignmentRepository(adapter, mapper, cacheService, logger);
        });


        services.AddScoped<IProductVariantRepository, ProductVariantRepository>(provider => {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<ProductVariantRepository>>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new ProductVariantRepository(adapter, mapper, cacheService, logger);
        });

        services.AddScoped<ICartRepository, CartRepository>(provider =>
        {
            var adapter = provider.GetRequiredService<DataAccessAdapter>();
            var mapper = provider.GetRequiredService<IMapper>();
            var logger = provider.GetRequiredService<ILogger<CartRepository>>();
            var unitOfWorkService = provider.GetRequiredService<IUnitOfWorkService>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new CartRepository(adapter, mapper, unitOfWorkService,  cacheService, logger);
        });

        

        // Register existing services
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
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

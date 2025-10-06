using System.Data.Common;
using System.Diagnostics;
using System.Text;
using GroceryEcommerce.Infrastructure;
using GroceryEcommerce.Application.Mapping;
using GroceryEcommerce.Infrastructure.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Scalar.AspNetCore;
using SD.LLBLGen.Pro.DQE.PostgreSql;
using SD.LLBLGen.Pro.ORMSupportClasses;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Đăng ký DbProviderFactory với .NET
        DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);

        // ⚡ Cấu hình LLBLGen DQE cho PostgreSQL (RuntimeConfiguration)
        RuntimeConfiguration.ConfigureDQE<PostgreSqlDQEConfiguration>(c =>
        {
            c.AddDbProviderFactory(typeof(NpgsqlFactory)); // dùng provider Npgsql
            c.SetTraceLevel(TraceLevel.Verbose); // bật log (optional)
        });


        // Clean Architecture layers
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AuthProfile>();
            cfg.AddProfile<CatalogProfile>();
            cfg.AddProfile<CartProfile>();
            cfg.AddProfile<SalesProfile>();
            cfg.AddProfile<InventoryProfile>();
            cfg.AddProfile<MarketingProfile>();
            cfg.AddProfile<ReviewsProfile>();
            cfg.AddProfile<SystemProfile>();
            cfg.AddProfile<AuthMappingProfile>();
            cfg.AddProfile<CartMappingProfile>();
            cfg.AddProfile<CatalogMappingProfile>();
            cfg.AddProfile<InventoryMappingProfile>();
            cfg.AddProfile<MarketingMappingProfile>();
            cfg.AddProfile<ReviewsMappingProfile>();
            cfg.AddProfile<SalesMappingProfile>();
            cfg.AddProfile<SystemMappingProfile>();
        });

        // Register all assemblies starting with "GroceryEcommerce"
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name!.StartsWith("GroceryEcommerce"))
            .ToArray();

        builder.Services.AddMediatR(cfg =>
        {
            foreach (var assembly in assemblies)
            {
                cfg.RegisterServicesFromAssembly(assembly);
            }
        });
    
        // JWT Configuration
        var jwtSettings = builder.Configuration.GetSection("JWT");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = Encoding.ASCII.GetBytes(secretKey);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization();

        // CORS

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://localhost:7129")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

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
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NSwag.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });
        // builder.Services.AddOpenApi();
        builder.Services.AddOpenApiDocument();



        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
        {
            // Tăng giới hạn độ sâu để tránh lỗi JsonSchemaExporterDepthTooLarge
            opts.SerializerOptions.MaxDepth = 256; // Tăng từ 64 lên 128

            // Bỏ qua vòng tham chiếu để tránh đệ quy vô hạn khi export schema/serialize
            opts.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });



        builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
        });
        
        builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
        });

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
                policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://localhost:7129", "http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        // PORT 
        var urls = builder.Configuration["Urls"];
        if (!string.IsNullOrEmpty(urls))
        {
            builder.WebHost.UseUrls(urls);
        }

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseOpenApi();
            app.MapScalarApiReference();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

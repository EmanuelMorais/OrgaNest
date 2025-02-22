using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrgaNestApi.Features.Categories;
using OrgaNestApi.Features.Expenses;
using OrgaNestApi.Features.Shopping;
using OrgaNestApi.Features.Users;
using OrgaNestApi.Infrastructure.Auth;
using OrgaNestApi.Infrastructure.Database;
using OrgaNestApi.Infrastructure.Middleware;

namespace OrgaNestApi.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IShoppingListService, ShoppingListService>();
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
    }

    public static void AddDbContexts(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlite(connectionString));
    }

    public static void AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddJwtAuthentication(this IServiceCollection services, string issuer, string audience,
        string secretKey)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)
                    )
                };
            });
    }

    public static void AddGoogleAuthentication(this IServiceCollection services, string clientId, string clientSecret)
    {
        services.AddAuthentication().AddGoogle(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
        });
    }

    public static void UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
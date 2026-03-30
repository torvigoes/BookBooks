using BookBooks.Domain.Interfaces;
using BookBooks.Infrastructure.Authentication;
using BookBooks.Infrastructure.Persistence;
using BookBooks.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookBooks.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookBooks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add Repositories
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IBookListRepository, BookListRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Setup Identity
        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Configure JWT Authentication
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions?.Issuer,
                    ValidAudience = jwtOptions?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions?.SecretKey ?? throw new InvalidOperationException("JWT SecretKey is missing from configuration.")))
                };
            });

        services.AddAuthorization();

        return services;
    }
}

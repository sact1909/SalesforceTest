using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Interfaces;
using SalesforceTest.Infrastructure.Persistence;
using SalesforceTest.Infrastructure.Repositories;
using SalesforceTest.Infrastructure.Services;

namespace SalesforceTest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=salesforcetest.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<ISalesforceConnectionRepository, SalesforceConnectionRepository>();
        services.AddScoped<ISalesforceObjectCacheRepository, SalesforceObjectCacheRepository>();
        services.AddHttpClient<ISalesforceOAuthService, SalesforceOAuthService>();
        services.AddHttpClient<ISalesforceDataService, SalesforceDataService>();

        return services;
    }
}

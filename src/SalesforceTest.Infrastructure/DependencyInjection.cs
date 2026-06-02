using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Infrastructure.Services;

namespace SalesforceTest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeService, DateTimeService>();

        return services;
    }
}

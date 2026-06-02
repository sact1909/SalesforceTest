using Microsoft.Extensions.DependencyInjection;
using SalesforceTest.Application.Features.Auth;

namespace SalesforceTest.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginService>();
        return services;
    }
}

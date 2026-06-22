using Microsoft.Extensions.DependencyInjection;
using SalesforceTest.Application.Features.Auth;
using SalesforceTest.Application.Features.Salesforce;

namespace SalesforceTest.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginService>();
        services.AddScoped<GetAuthorizationUrlService>();
        services.AddScoped<HandleOAuthCallbackService>();
        services.AddScoped<GetSalesforceConnectionService>();
        services.AddScoped<DisconnectSalesforceService>();
        return services;
    }
}

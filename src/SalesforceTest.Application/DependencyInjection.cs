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
        services.AddScoped<GetOrdersService>();
        services.AddScoped<GetInvoicesService>();
        services.AddScoped<GetAccountsService>();
        services.AddScoped<GetContactsService>();
        services.AddScoped<GetAvailableObjectsService>();
        services.AddScoped<GetObjectRecordsService>();
        services.AddScoped<RescanObjectsService>();
        services.AddScoped<RefreshObjectCountService>();
        return services;
    }
}

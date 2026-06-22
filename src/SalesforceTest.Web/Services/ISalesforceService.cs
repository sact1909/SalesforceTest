using SalesforceTest.Web.Models;

namespace SalesforceTest.Web.Services;

public interface ISalesforceService
{
    Task<string?> GetAuthorizationUrlAsync(CancellationToken cancellationToken = default);
    Task<SalesforceConnectionModel?> GetConnectionAsync(CancellationToken cancellationToken = default);
    Task<bool> DisconnectAsync(CancellationToken cancellationToken = default);
}

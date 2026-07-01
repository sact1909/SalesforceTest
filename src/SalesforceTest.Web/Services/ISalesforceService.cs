using SalesforceTest.Web.Models;

namespace SalesforceTest.Web.Services;

public interface ISalesforceService
{
    Task<string?> GetAuthorizationUrlAsync(CancellationToken cancellationToken = default);
    Task<SalesforceConnectionModel?> GetConnectionAsync(CancellationToken cancellationToken = default);
    Task<bool> DisconnectAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceOrderModel>?> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceInvoiceModel>?> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceAccountModel>?> GetAccountsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceContactModel>?> GetContactsAsync(CancellationToken cancellationToken = default);
    Task<CachedObjectsResult?> GetAvailableObjectsAsync(CancellationToken cancellationToken = default);
    Task<CachedObjectsResult?> RescanObjectsAsync(CancellationToken cancellationToken = default);
    Task<int?> RefreshObjectCountAsync(string objectApiName, CancellationToken cancellationToken = default);
    Task<SalesforceObjectRecordsModel?> GetObjectRecordsAsync(string objectApiName, CancellationToken cancellationToken = default);
}

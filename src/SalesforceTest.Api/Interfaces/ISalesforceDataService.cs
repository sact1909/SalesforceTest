using SalesforceTest.Api.DTOs.Salesforce;

namespace SalesforceTest.Api.Interfaces;

public interface ISalesforceDataService
{
    Task<IReadOnlyList<SalesforceOrderDto>> GetOrdersAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceInvoiceDto>> GetInvoicesAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceAccountDto>> GetAccountsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceContactDto>> GetContactsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceObjectInfoDto>> GetAvailableObjectsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<SalesforceObjectRecordsDto> GetObjectRecordsAsync(string instanceUrl, string accessToken, string objectApiName, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default);
    Task<int> GetObjectCountAsync(string instanceUrl, string accessToken, string objectApiName, CancellationToken cancellationToken = default);
}

using SalesforceTest.Application.DTOs.Salesforce;

namespace SalesforceTest.Application.Interfaces;

public interface ISalesforceDataService
{
    Task<IReadOnlyList<SalesforceOrderDto>> GetOrdersAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceInvoiceDto>> GetInvoicesAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceAccountDto>> GetAccountsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesforceContactDto>> GetContactsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
}

using SalesforceTest.Application.DTOs.Salesforce;
using SalesforceTest.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SalesforceTest.Infrastructure.Services;

public sealed class SalesforceDataService : ISalesforceDataService
{
    private readonly HttpClient _httpClient;

    public SalesforceDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<SalesforceOrderDto>> GetOrdersAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default)
    {
        const string soql = "SELECT Id, OrderNumber, Status, TotalAmount, Account.Name, EffectiveDate, Description FROM Order ORDER BY EffectiveDate DESC NULLS LAST LIMIT 200";
        var url = $"{instanceUrl}/services/data/v59.0/query?q={Uri.EscapeDataString(soql)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Salesforce API error ({(int)response.StatusCode}): {errorBody}");
        }

        var result = await response.Content.ReadFromJsonAsync<SoqlQueryResult>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty response from Salesforce SOQL query.");

        return result.Records
            .Select(r => new SalesforceOrderDto(
                Id: r.Id,
                OrderNumber: r.OrderNumber,
                Status: r.Status,
                TotalAmount: r.TotalAmount,
                AccountName: r.Account?.Name,
                EffectiveDate: r.EffectiveDate,
                Description: r.Description))
            .ToList();
    }

    public async Task<IReadOnlyList<SalesforceInvoiceDto>> GetInvoicesAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default)
    {
        const string soql = "SELECT Id, DocumentNumber, Status, TotalAmount, TotalAmountWithTax, Balance, InvoiceDate, DueDate, BillToContact.Name, Description FROM Invoice ORDER BY InvoiceDate DESC NULLS LAST LIMIT 200";
        var url = $"{instanceUrl}/services/data/v59.0/query?q={Uri.EscapeDataString(soql)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Salesforce API error ({(int)response.StatusCode}): {errorBody}");
        }

        var result = await response.Content.ReadFromJsonAsync<SoqlInvoiceQueryResult>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty response from Salesforce SOQL query.");

        return result.Records
            .Select(r => new SalesforceInvoiceDto(
                Id: r.Id,
                InvoiceNumber: r.DocumentNumber,
                Status: r.Status,
                TotalAmount: r.TotalAmount,
                TotalAmountWithTax: r.TotalAmountWithTax,
                Balance: r.Balance,
                InvoiceDate: r.InvoiceDate,
                DueDate: r.DueDate,
                BillToName: r.BillToContact?.Name,
                Description: r.Description))
            .ToList();
    }

    public async Task<IReadOnlyList<SalesforceAccountDto>> GetAccountsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default)
    {
        const string soql = "SELECT Id, Name, Type, Industry, Phone, Website, BillingCity, BillingCountry, NumberOfEmployees FROM Account ORDER BY Name ASC LIMIT 200";
        var url = $"{instanceUrl}/services/data/v59.0/query?q={Uri.EscapeDataString(soql)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Salesforce API error ({(int)response.StatusCode}): {errorBody}");
        }

        var result = await response.Content.ReadFromJsonAsync<SoqlAccountQueryResult>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty response from Salesforce SOQL query.");

        return result.Records
            .Select(r => new SalesforceAccountDto(
                Id: r.Id,
                Name: r.Name,
                Type: r.Type,
                Industry: r.Industry,
                Phone: r.Phone,
                Website: r.Website,
                BillingCity: r.BillingCity,
                BillingCountry: r.BillingCountry,
                NumberOfEmployees: r.NumberOfEmployees))
            .ToList();
    }

    public async Task<IReadOnlyList<SalesforceContactDto>> GetContactsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default)
    {
        const string soql = "SELECT Id, FirstName, LastName, Email, Phone, Title, Account.Name, MailingCity, MailingCountry FROM Contact ORDER BY LastName ASC LIMIT 200";
        var url = $"{instanceUrl}/services/data/v59.0/query?q={Uri.EscapeDataString(soql)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Salesforce API error ({(int)response.StatusCode}): {errorBody}");
        }

        var result = await response.Content.ReadFromJsonAsync<SoqlContactQueryResult>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty response from Salesforce SOQL query.");

        return result.Records
            .Select(r => new SalesforceContactDto(
                Id: r.Id,
                FirstName: r.FirstName,
                LastName: r.LastName,
                Email: r.Email,
                Phone: r.Phone,
                Title: r.Title,
                AccountName: r.Account?.Name,
                MailingCity: r.MailingCity,
                MailingCountry: r.MailingCountry))
            .ToList();
    }

    private sealed record SoqlQueryResult(
        [property: JsonPropertyName("records")] List<OrderRecord> Records
    );

    private sealed record OrderRecord(
        [property: JsonPropertyName("Id")] string Id,
        [property: JsonPropertyName("OrderNumber")] string OrderNumber,
        [property: JsonPropertyName("Status")] string Status,
        [property: JsonPropertyName("TotalAmount")] decimal? TotalAmount,
        [property: JsonPropertyName("Account")] AccountRef? Account,
        [property: JsonPropertyName("EffectiveDate")] DateTime? EffectiveDate,
        [property: JsonPropertyName("Description")] string? Description
    );

    private sealed record AccountRef(
        [property: JsonPropertyName("Name")] string Name
    );

    private sealed record SoqlInvoiceQueryResult(
        [property: JsonPropertyName("records")] List<InvoiceRecord> Records
    );

    private sealed record InvoiceRecord(
        [property: JsonPropertyName("Id")] string Id,
        [property: JsonPropertyName("DocumentNumber")] string? DocumentNumber,
        [property: JsonPropertyName("Status")] string Status,
        [property: JsonPropertyName("TotalAmount")] decimal? TotalAmount,
        [property: JsonPropertyName("TotalAmountWithTax")] decimal? TotalAmountWithTax,
        [property: JsonPropertyName("Balance")] decimal? Balance,
        [property: JsonPropertyName("InvoiceDate")] DateTime? InvoiceDate,
        [property: JsonPropertyName("DueDate")] DateTime? DueDate,
        [property: JsonPropertyName("BillToContact")] ContactRef? BillToContact,
        [property: JsonPropertyName("Description")] string? Description
    );

    private sealed record ContactRef(
        [property: JsonPropertyName("Name")] string Name
    );

    private sealed record SoqlAccountQueryResult(
        [property: JsonPropertyName("records")] List<AccountRecord> Records
    );

    private sealed record AccountRecord(
        [property: JsonPropertyName("Id")] string Id,
        [property: JsonPropertyName("Name")] string Name,
        [property: JsonPropertyName("Type")] string? Type,
        [property: JsonPropertyName("Industry")] string? Industry,
        [property: JsonPropertyName("Phone")] string? Phone,
        [property: JsonPropertyName("Website")] string? Website,
        [property: JsonPropertyName("BillingCity")] string? BillingCity,
        [property: JsonPropertyName("BillingCountry")] string? BillingCountry,
        [property: JsonPropertyName("NumberOfEmployees")] int? NumberOfEmployees
    );

    private sealed record SoqlContactQueryResult(
        [property: JsonPropertyName("records")] List<ContactRecord> Records
    );

    private sealed record ContactRecord(
        [property: JsonPropertyName("Id")] string Id,
        [property: JsonPropertyName("FirstName")] string? FirstName,
        [property: JsonPropertyName("LastName")] string LastName,
        [property: JsonPropertyName("Email")] string? Email,
        [property: JsonPropertyName("Phone")] string? Phone,
        [property: JsonPropertyName("Title")] string? Title,
        [property: JsonPropertyName("Account")] AccountRef? Account,
        [property: JsonPropertyName("MailingCity")] string? MailingCity,
        [property: JsonPropertyName("MailingCountry")] string? MailingCountry
    );
}

using SalesforceTest.Application.DTOs.Salesforce;
using SalesforceTest.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
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

    public async Task<IReadOnlyList<SalesforceObjectInfoDto>> GetAvailableObjectsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default)
    {
        // Step 1: get all standard queryable objects
        var globalDescribe = await SendAsync<GlobalDescribeResult>(
            instanceUrl, accessToken, $"{instanceUrl}/services/data/v59.0/sobjects", cancellationToken);

        var candidates = globalDescribe.SObjects
            .Where(o => o.Queryable && !o.DeprecatedAndHidden && o.CustomSetting == false && !o.ApiName.EndsWith("__c"))
            .ToList();

        // Step 2: count records in parallel batches of 20
        var withRecords = new System.Collections.Concurrent.ConcurrentBag<(GlobalDescribeSObject obj, int count)>();

        var semaphore = new SemaphoreSlim(20, 20);
        var tasks = candidates.Select(async obj =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var countSoql = $"SELECT COUNT() FROM {obj.ApiName}";
                var url = $"{instanceUrl}/services/data/v59.0/query?q={Uri.EscapeDataString(countSoql)}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode) return;

                var result = await response.Content.ReadFromJsonAsync<CountQueryResult>(cancellationToken: cancellationToken);
                if (result is not null && result.TotalSize > 0)
                    withRecords.Add((obj, result.TotalSize));
            }
            catch { /* skip objects that can't be queried */ }
            finally { semaphore.Release(); }
        });

        await Task.WhenAll(tasks);

        // Step 3: for each object with records, fetch its describe to get fields
        var results = new List<SalesforceObjectInfoDto>();
        foreach (var (obj, count) in withRecords.OrderBy(x => x.obj.Label))
        {
            try
            {
                var describeUrl = $"{instanceUrl}/services/data/v59.0/sobjects/{obj.ApiName}/describe";
                var describe = await SendAsync<ObjectDescribeResult>(instanceUrl, accessToken, describeUrl, cancellationToken);

                var fields = describe.Fields
                    .Where(f => f.Type != "address" && f.Type != "location")
                    .Select(f => new SalesforceFieldInfoDto(f.Name, f.Label, f.Type))
                    .ToList();

                results.Add(new SalesforceObjectInfoDto(obj.ApiName, obj.Label, obj.LabelPlural, count, fields));
            }
            catch { /* skip if describe fails */ }
        }

        return results;
    }

    public async Task<SalesforceObjectRecordsDto> GetObjectRecordsAsync(string instanceUrl, string accessToken, string objectApiName, CancellationToken cancellationToken = default)
    {
        // Describe to get fields
        var describeUrl = $"{instanceUrl}/services/data/v59.0/sobjects/{objectApiName}/describe";
        var describe = await SendAsync<ObjectDescribeResult>(instanceUrl, accessToken, describeUrl, cancellationToken);

        var fields = describe.Fields
            .Where(f => f.Type != "address" && f.Type != "location" && f.Type != "base64")
            .Select(f => new SalesforceFieldInfoDto(f.Name, f.Label, f.Type))
            .Take(20) // cap columns to keep queries manageable
            .ToList();

        var fieldList = string.Join(", ", fields.Select(f => f.ApiName));
        var soql = $"SELECT {fieldList} FROM {objectApiName} LIMIT 200";
        var queryUrl = $"{instanceUrl}/services/data/v59.0/query?q={Uri.EscapeDataString(soql)}";

        var request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Salesforce API error ({(int)response.StatusCode}): {errorBody}");
        }

        var raw = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var records = new List<Dictionary<string, object?>>();

        if (raw.TryGetProperty("records", out var recordsEl))
        {
            foreach (var record in recordsEl.EnumerateArray())
            {
                var dict = new Dictionary<string, object?>();
                foreach (var field in fields)
                {
                    if (record.TryGetProperty(field.ApiName, out var val))
                        dict[field.ApiName] = val.ValueKind == JsonValueKind.Null ? null : val.ToString();
                    else
                        dict[field.ApiName] = null;
                }
                records.Add(dict);
            }
        }

        return new SalesforceObjectRecordsDto(objectApiName, fields, records.Cast<IReadOnlyDictionary<string, object?>>().ToList());
    }

    public async Task<int> GetObjectCountAsync(string instanceUrl, string accessToken, string objectApiName, CancellationToken cancellationToken = default)
    {
        var soql = $"SELECT COUNT() FROM {objectApiName}";
        var url = $"{instanceUrl}/services/data/v59.0/query?q={Uri.EscapeDataString(soql)}";
        var result = await SendAsync<CountQueryResult>(instanceUrl, accessToken, url, cancellationToken);
        return result.TotalSize;
    }

    private async Task<T> SendAsync<T>(string instanceUrl, string accessToken, string url, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Salesforce API error ({(int)response.StatusCode}): {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty response from Salesforce.");
    }

    private sealed record GlobalDescribeResult(
        [property: JsonPropertyName("sobjects")] List<GlobalDescribeSObject> SObjects
    );

    private sealed record GlobalDescribeSObject(
        [property: JsonPropertyName("name")] string ApiName,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("labelPlural")] string LabelPlural,
        [property: JsonPropertyName("queryable")] bool Queryable,
        [property: JsonPropertyName("deprecatedAndHidden")] bool DeprecatedAndHidden,
        [property: JsonPropertyName("customSetting")] bool CustomSetting
    );

    private sealed record CountQueryResult(
        [property: JsonPropertyName("totalSize")] int TotalSize
    );

    private sealed record ObjectDescribeResult(
        [property: JsonPropertyName("fields")] List<DescribeField> Fields
    );

    private sealed record DescribeField(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("type")] string Type
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

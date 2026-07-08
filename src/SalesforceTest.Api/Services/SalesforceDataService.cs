using SalesforceTest.Api.DTOs.Salesforce;
using SalesforceTest.Api.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SalesforceTest.Api.Services;

internal sealed class SalesforceDataService : ISalesforceDataService
{
    private readonly HttpClient _httpClient;

    public SalesforceDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<SalesforceObjectInfoDto>> GetAvailableObjectsAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default)
    {
        var globalDescribe = await SendAsync<GlobalDescribeResult>(
            instanceUrl, accessToken, $"{instanceUrl}/services/data/v59.0/sobjects", cancellationToken);

        var candidates = globalDescribe.SObjects
            .Where(o => o.Queryable && !o.DeprecatedAndHidden && o.CustomSetting == false && !o.ApiName.EndsWith("__c"))
            .ToList();

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

    public async Task<SalesforceObjectRecordsDto> GetObjectRecordsAsync(string instanceUrl, string accessToken, string objectApiName, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var describeUrl = $"{instanceUrl}/services/data/v59.0/sobjects/{objectApiName}/describe";
        var describe = await SendAsync<ObjectDescribeResult>(instanceUrl, accessToken, describeUrl, cancellationToken);

        var fields = describe.Fields
            .Where(f => f.Type != "address" && f.Type != "location" && f.Type != "base64")
            .Select(f => new SalesforceFieldInfoDto(f.Name, f.Label, f.Type))
            .Take(20)
            .ToList();

        var fieldList = string.Join(", ", fields.Select(f => f.ApiName));

        var whereClauses = new List<string>();
        if (from.HasValue)
            whereClauses.Add($"LastModifiedDate >= {from.Value.UtcDateTime:yyyy-MM-ddTHH:mm:ssZ}");
        if (to.HasValue)
            whereClauses.Add($"LastModifiedDate <= {to.Value.UtcDateTime:yyyy-MM-ddTHH:mm:ssZ}");

        var where = whereClauses.Count > 0 ? $" WHERE {string.Join(" AND ", whereClauses)}" : string.Empty;
        var soql = $"SELECT {fieldList} FROM {objectApiName}{where} LIMIT 200";
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

}

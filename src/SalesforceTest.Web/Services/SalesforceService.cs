using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SalesforceTest.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SalesforceTest.Web.Services;

public sealed class SalesforceService : ISalesforceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly ILogger<SalesforceService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private const string SessionKey = "auth_user";

    public SalesforceService(
        IHttpClientFactory httpClientFactory,
        ProtectedSessionStorage sessionStorage,
        ILogger<SalesforceService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _sessionStorage = sessionStorage;
        _logger = logger;
    }

    public async Task<string?> GetAuthorizationUrlAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client is null) return null;

            var response = await client.GetFromJsonAsync<JsonElement>("api/salesforce/authorize", JsonOptions, cancellationToken);
            return response.GetString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Salesforce authorization URL.");
            return null;
        }
    }

    public async Task<SalesforceConnectionModel?> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client is null) return null;

            return await client.GetFromJsonAsync<SalesforceConnectionModel>("api/salesforce/connection", JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Salesforce connection.");
            return null;
        }
    }

    public async Task<IReadOnlyList<SalesforceOrderModel>?> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client is null) return null;

            var response = await client.GetAsync("api/salesforce/orders", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                var message = body.TryGetProperty("message", out var msg) ? msg.GetString() : null;
                throw new InvalidOperationException(message ?? $"API error {(int)response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<List<SalesforceOrderModel>>(JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Salesforce orders.");
            throw;
        }
    }

    public async Task<IReadOnlyList<SalesforceInvoiceModel>?> GetInvoicesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client is null) return null;

            var response = await client.GetAsync("api/salesforce/invoices", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                var message = body.TryGetProperty("message", out var msg) ? msg.GetString() : null;
                throw new InvalidOperationException(message ?? $"API error {(int)response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<List<SalesforceInvoiceModel>>(JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Salesforce invoices.");
            throw;
        }
    }

    public async Task<IReadOnlyList<SalesforceAccountModel>?> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client is null) return null;

            var response = await client.GetAsync("api/salesforce/accounts", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                var message = body.TryGetProperty("message", out var msg) ? msg.GetString() : null;
                throw new InvalidOperationException(message ?? $"API error {(int)response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<List<SalesforceAccountModel>>(JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Salesforce accounts.");
            throw;
        }
    }

    public async Task<IReadOnlyList<SalesforceContactModel>?> GetContactsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client is null) return null;

            var response = await client.GetAsync("api/salesforce/contacts", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                var message = body.TryGetProperty("message", out var msg) ? msg.GetString() : null;
                throw new InvalidOperationException(message ?? $"API error {(int)response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<List<SalesforceContactModel>>(JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Salesforce contacts.");
            throw;
        }
    }

    public async Task<bool> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client is null) return false;

            var response = await client.DeleteAsync("api/salesforce/connection", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnect Salesforce account.");
            return false;
        }
    }

    private async Task<HttpClient?> CreateAuthenticatedClientAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<AuthUser>(SessionKey);
            if (!result.Success || result.Value is null) return null;

            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Value.Token);
            return client;
        }
        catch
        {
            return null;
        }
    }
}

using Microsoft.Extensions.Configuration;
using SalesforceTest.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace SalesforceTest.Infrastructure.Services;

public sealed class SalesforceOAuthService : ISalesforceOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly string _authBaseUrl;

    public SalesforceOAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _clientId = configuration["Salesforce:ClientId"]
            ?? throw new InvalidOperationException("Salesforce:ClientId is not configured.");
        _clientSecret = configuration["Salesforce:ClientSecret"]
            ?? throw new InvalidOperationException("Salesforce:ClientSecret is not configured.");
        _redirectUri = configuration["Salesforce:RedirectUri"]
            ?? throw new InvalidOperationException("Salesforce:RedirectUri is not configured.");
        _authBaseUrl = configuration["Salesforce:AuthBaseUrl"] ?? "https://login.salesforce.com";
    }

    public string BuildAuthorizationUrl(string state)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["response_type"] = "code";
        query["client_id"] = _clientId;
        query["redirect_uri"] = _redirectUri;
        query["state"] = state;
        query["scope"] = "api refresh_token offline_access";
        return $"{_authBaseUrl}/services/oauth2/authorize?{query}";
    }

    public async Task<SalesforceTokenResponse> ExchangeCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["redirect_uri"] = _redirectUri,
            ["code"] = code
        });

        var response = await _httpClient.PostAsync($"{_authBaseUrl}/services/oauth2/token", body, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<SalesforceTokenJsonResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty token response from Salesforce.");

        return new SalesforceTokenResponse(
            AccessToken: json.AccessToken,
            RefreshToken: json.RefreshToken,
            InstanceUrl: json.InstanceUrl,
            Id: json.Id,
            IssuedAt: long.Parse(json.IssuedAt)
        );
    }

    public async Task<SalesforceUserInfo> GetUserInfoAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{instanceUrl}/services/oauth2/userinfo");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<SalesforceUserInfoJson>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Empty user info response from Salesforce.");

        return new SalesforceUserInfo(
            UserId: json.UserId,
            OrganizationId: json.OrganizationId,
            DisplayName: json.Name,
            Email: json.Email
        );
    }

    private sealed record SalesforceTokenJsonResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("instance_url")] string InstanceUrl,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("issued_at")] string IssuedAt
    );

    private sealed record SalesforceUserInfoJson(
        [property: JsonPropertyName("user_id")] string UserId,
        [property: JsonPropertyName("organization_id")] string OrganizationId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("email")] string Email
    );
}

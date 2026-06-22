namespace SalesforceTest.Application.Interfaces;

public interface ISalesforceOAuthService
{
    string BuildAuthorizationUrl(string state);
    Task<SalesforceTokenResponse> ExchangeCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<SalesforceUserInfo> GetUserInfoAsync(string instanceUrl, string accessToken, CancellationToken cancellationToken = default);
}

public sealed record SalesforceTokenResponse(
    string AccessToken,
    string RefreshToken,
    string InstanceUrl,
    string Id,
    long IssuedAt
);

public sealed record SalesforceUserInfo(
    string UserId,
    string OrganizationId,
    string DisplayName,
    string Email
);

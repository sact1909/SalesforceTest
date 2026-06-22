using SalesforceTest.Domain.Common;

namespace SalesforceTest.Domain.Entities;

public sealed class SalesforceConnection : BaseEntity
{
    public Guid UserId { get; private set; }
    public string InstanceUrl { get; private set; } = string.Empty;
    public string AccessToken { get; private set; } = string.Empty;
    public string RefreshToken { get; private set; } = string.Empty;
    public string SalesforceUserId { get; private set; } = string.Empty;
    public string OrganizationId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime TokenExpiresAt { get; private set; }

    public User User { get; private set; } = null!;

    private SalesforceConnection() { }

    public static SalesforceConnection Create(
        Guid userId,
        string instanceUrl,
        string accessToken,
        string refreshToken,
        string salesforceUserId,
        string organizationId,
        string displayName,
        string email,
        DateTime tokenExpiresAt)
    {
        return new SalesforceConnection
        {
            UserId = userId,
            InstanceUrl = instanceUrl,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            SalesforceUserId = salesforceUserId,
            OrganizationId = organizationId,
            DisplayName = displayName,
            Email = email,
            TokenExpiresAt = tokenExpiresAt
        };
    }

    public void UpdateTokens(string accessToken, string refreshToken, DateTime tokenExpiresAt)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        TokenExpiresAt = tokenExpiresAt;
    }
}

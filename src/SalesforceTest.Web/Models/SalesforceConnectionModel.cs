namespace SalesforceTest.Web.Models;

public sealed record SalesforceConnectionModel(
    string SalesforceUserId,
    string OrganizationId,
    string DisplayName,
    string Email,
    string InstanceUrl,
    DateTime TokenExpiresAt,
    bool IsConnected
);

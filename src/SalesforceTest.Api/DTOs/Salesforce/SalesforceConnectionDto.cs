namespace SalesforceTest.Api.DTOs.Salesforce;

public sealed record SalesforceConnectionDto(
    string SalesforceUserId,
    string OrganizationId,
    string DisplayName,
    string Email,
    string InstanceUrl,
    DateTime TokenExpiresAt,
    bool IsConnected
);

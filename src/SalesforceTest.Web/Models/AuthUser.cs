namespace SalesforceTest.Web.Models;

public sealed record AuthUser(
    string Username,
    string Email,
    string FullName,
    string Token,
    DateTime ExpiresAt
);

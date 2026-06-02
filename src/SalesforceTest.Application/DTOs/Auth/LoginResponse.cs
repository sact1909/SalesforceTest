namespace SalesforceTest.Application.DTOs.Auth;

public sealed record LoginResponse(
    string Token,
    string Username,
    string Email,
    string FullName,
    DateTime ExpiresAt
);

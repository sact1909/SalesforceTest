namespace SalesforceTest.Web.Models;

public sealed record SalesforceAccountModel(
    string Id,
    string Name,
    string? Type,
    string? Industry,
    string? Phone,
    string? Website,
    string? BillingCity,
    string? BillingCountry,
    int? NumberOfEmployees
);

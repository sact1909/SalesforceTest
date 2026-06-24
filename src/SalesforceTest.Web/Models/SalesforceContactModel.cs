namespace SalesforceTest.Web.Models;

public sealed record SalesforceContactModel(
    string Id,
    string? FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Title,
    string? AccountName,
    string? MailingCity,
    string? MailingCountry
);

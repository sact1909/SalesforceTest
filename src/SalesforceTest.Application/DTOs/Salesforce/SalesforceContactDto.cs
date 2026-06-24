using System.Text.Json.Serialization;

namespace SalesforceTest.Application.DTOs.Salesforce;

public sealed record SalesforceContactDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("firstName")] string? FirstName,
    [property: JsonPropertyName("lastName")] string LastName,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("phone")] string? Phone,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("accountName")] string? AccountName,
    [property: JsonPropertyName("mailingCity")] string? MailingCity,
    [property: JsonPropertyName("mailingCountry")] string? MailingCountry
);

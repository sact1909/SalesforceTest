using System.Text.Json.Serialization;

namespace SalesforceTest.Application.DTOs.Salesforce;

public sealed record SalesforceAccountDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("industry")] string? Industry,
    [property: JsonPropertyName("phone")] string? Phone,
    [property: JsonPropertyName("website")] string? Website,
    [property: JsonPropertyName("billingCity")] string? BillingCity,
    [property: JsonPropertyName("billingCountry")] string? BillingCountry,
    [property: JsonPropertyName("numberOfEmployees")] int? NumberOfEmployees
);

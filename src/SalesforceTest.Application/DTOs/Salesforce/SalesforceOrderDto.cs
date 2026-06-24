using System.Text.Json.Serialization;

namespace SalesforceTest.Application.DTOs.Salesforce;

public sealed record SalesforceOrderDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("orderNumber")] string OrderNumber,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("totalAmount")] decimal? TotalAmount,
    [property: JsonPropertyName("accountName")] string? AccountName,
    [property: JsonPropertyName("effectiveDate")] DateTime? EffectiveDate,
    [property: JsonPropertyName("description")] string? Description
);

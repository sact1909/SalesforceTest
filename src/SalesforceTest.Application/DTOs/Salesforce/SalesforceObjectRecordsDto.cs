using System.Text.Json.Serialization;

namespace SalesforceTest.Application.DTOs.Salesforce;

public sealed record SalesforceObjectRecordsDto(
    [property: JsonPropertyName("objectApiName")] string ObjectApiName,
    [property: JsonPropertyName("fields")] IReadOnlyList<SalesforceFieldInfoDto> Fields,
    [property: JsonPropertyName("records")] IReadOnlyList<IReadOnlyDictionary<string, object?>> Records
);

using System.Text.Json.Serialization;

namespace SalesforceTest.Application.DTOs.Salesforce;

public sealed record SalesforceObjectInfoDto(
    [property: JsonPropertyName("apiName")] string ApiName,
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("labelPlural")] string LabelPlural,
    [property: JsonPropertyName("recordCount")] int RecordCount,
    [property: JsonPropertyName("fields")] IReadOnlyList<SalesforceFieldInfoDto> Fields
);

public sealed record SalesforceFieldInfoDto(
    [property: JsonPropertyName("apiName")] string ApiName,
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("type")] string Type
);

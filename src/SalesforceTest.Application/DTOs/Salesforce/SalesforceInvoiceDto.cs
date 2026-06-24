using System.Text.Json.Serialization;

namespace SalesforceTest.Application.DTOs.Salesforce;

public sealed record SalesforceInvoiceDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("invoiceNumber")] string? InvoiceNumber,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("totalAmount")] decimal? TotalAmount,
    [property: JsonPropertyName("totalAmountWithTax")] decimal? TotalAmountWithTax,
    [property: JsonPropertyName("balance")] decimal? Balance,
    [property: JsonPropertyName("invoiceDate")] DateTime? InvoiceDate,
    [property: JsonPropertyName("dueDate")] DateTime? DueDate,
    [property: JsonPropertyName("billToName")] string? BillToName,
    [property: JsonPropertyName("description")] string? Description
);

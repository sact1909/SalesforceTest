namespace SalesforceTest.Web.Models;

public sealed record SalesforceInvoiceModel(
    string Id,
    string? InvoiceNumber,
    string Status,
    decimal? TotalAmount,
    decimal? TotalAmountWithTax,
    decimal? Balance,
    DateTime? InvoiceDate,
    DateTime? DueDate,
    string? BillToName,
    string? Description
);

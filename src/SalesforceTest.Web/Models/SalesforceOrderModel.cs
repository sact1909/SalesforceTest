namespace SalesforceTest.Web.Models;

public sealed record SalesforceOrderModel(
    string Id,
    string OrderNumber,
    string Status,
    decimal? TotalAmount,
    string? AccountName,
    DateTime? EffectiveDate,
    string? Description
);

namespace SalesforceTest.Web.Models;

public sealed record SalesforceObjectRecordsModel(
    string ObjectApiName,
    IReadOnlyList<SalesforceFieldInfoModel> Fields,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Records
);

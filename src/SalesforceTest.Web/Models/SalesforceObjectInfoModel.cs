namespace SalesforceTest.Web.Models;

public sealed record SalesforceObjectInfoModel(
    string ApiName,
    string Label,
    string LabelPlural,
    int RecordCount,
    IReadOnlyList<SalesforceFieldInfoModel> Fields
);

public sealed record SalesforceFieldInfoModel(
    string ApiName,
    string Label,
    string Type
);

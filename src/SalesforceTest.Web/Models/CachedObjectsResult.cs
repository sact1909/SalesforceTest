namespace SalesforceTest.Web.Models;

public sealed record CachedObjectsResult(
    IReadOnlyList<SalesforceObjectInfoModel> Objects,
    DateTime LastScannedAt,
    bool FromCache
);

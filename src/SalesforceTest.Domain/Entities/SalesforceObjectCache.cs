using SalesforceTest.Domain.Common;

namespace SalesforceTest.Domain.Entities;

public sealed class SalesforceObjectCache : BaseEntity
{
    public Guid UserId { get; private set; }
    public string ApiName { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public string LabelPlural { get; private set; } = string.Empty;
    public int RecordCount { get; private set; }
    public string FieldsJson { get; private set; } = string.Empty;
    public DateTime LastScannedAt { get; private set; }

    private SalesforceObjectCache() { }

    public static SalesforceObjectCache Create(
        Guid userId,
        string apiName,
        string label,
        string labelPlural,
        int recordCount,
        string fieldsJson,
        DateTime lastScannedAt)
    {
        return new SalesforceObjectCache
        {
            UserId = userId,
            ApiName = apiName,
            Label = label,
            LabelPlural = labelPlural,
            RecordCount = recordCount,
            FieldsJson = fieldsJson,
            LastScannedAt = lastScannedAt
        };
    }

    public void UpdateCount(int recordCount)
    {
        RecordCount = recordCount;
        SetUpdatedAt();
    }

    public void UpdateAll(string label, string labelPlural, int recordCount, string fieldsJson, DateTime lastScannedAt)
    {
        Label = label;
        LabelPlural = labelPlural;
        RecordCount = recordCount;
        FieldsJson = fieldsJson;
        LastScannedAt = lastScannedAt;
        SetUpdatedAt();
    }
}

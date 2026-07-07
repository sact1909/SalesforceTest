using SalesforceTest.Api.Entities;

namespace SalesforceTest.Api.Interfaces;

public interface ISalesforceObjectCacheRepository
{
    Task<IReadOnlyList<SalesforceObjectCache>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SalesforceObjectCache?> GetByUserAndApiNameAsync(Guid userId, string apiName, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<SalesforceObjectCache> entities, CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

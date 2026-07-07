using SalesforceTest.Api.Entities;

namespace SalesforceTest.Api.Interfaces;

public interface ISalesforceConnectionRepository
{
    Task<SalesforceConnection?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(SalesforceConnection connection, CancellationToken cancellationToken = default);
    Task RemoveAsync(SalesforceConnection connection, CancellationToken cancellationToken = default);
}

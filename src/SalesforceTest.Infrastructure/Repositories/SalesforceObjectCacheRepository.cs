using Microsoft.EntityFrameworkCore;
using SalesforceTest.Domain.Entities;
using SalesforceTest.Domain.Interfaces;
using SalesforceTest.Infrastructure.Persistence;

namespace SalesforceTest.Infrastructure.Repositories;

public sealed class SalesforceObjectCacheRepository : ISalesforceObjectCacheRepository
{
    private readonly AppDbContext _context;

    public SalesforceObjectCacheRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<IReadOnlyList<SalesforceObjectCache>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => _context.SalesforceObjectCaches
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Label)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<SalesforceObjectCache>)t.Result, TaskContinuationOptions.ExecuteSynchronously);

    public Task<SalesforceObjectCache?> GetByUserAndApiNameAsync(Guid userId, string apiName, CancellationToken cancellationToken = default)
        => _context.SalesforceObjectCaches
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ApiName == apiName, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<SalesforceObjectCache> entities, CancellationToken cancellationToken = default)
        => await _context.SalesforceObjectCaches.AddRangeAsync(entities, cancellationToken);

    public async Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await _context.SalesforceObjectCaches
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
        _context.SalesforceObjectCaches.RemoveRange(existing);
    }
}

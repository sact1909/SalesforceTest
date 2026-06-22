using Microsoft.EntityFrameworkCore;
using SalesforceTest.Domain.Entities;
using SalesforceTest.Domain.Interfaces;
using SalesforceTest.Infrastructure.Persistence;

namespace SalesforceTest.Infrastructure.Repositories;

public sealed class SalesforceConnectionRepository : ISalesforceConnectionRepository
{
    private readonly AppDbContext _context;

    public SalesforceConnectionRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<SalesforceConnection?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => _context.SalesforceConnections.FirstOrDefaultAsync(sc => sc.UserId == userId, cancellationToken);

    public async Task AddAsync(SalesforceConnection connection, CancellationToken cancellationToken = default)
        => await _context.SalesforceConnections.AddAsync(connection, cancellationToken);

    public Task RemoveAsync(SalesforceConnection connection, CancellationToken cancellationToken = default)
    {
        _context.SalesforceConnections.Remove(connection);
        return Task.CompletedTask;
    }
}

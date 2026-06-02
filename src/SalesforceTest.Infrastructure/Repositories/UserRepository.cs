using Microsoft.EntityFrameworkCore;
using SalesforceTest.Domain.Entities;
using SalesforceTest.Domain.Interfaces;
using SalesforceTest.Infrastructure.Persistence;

namespace SalesforceTest.Infrastructure.Repositories;

internal sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(u => u.Username == username.ToLowerInvariant(), cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(u => u.Username == username.ToLowerInvariant(), cancellationToken);
}

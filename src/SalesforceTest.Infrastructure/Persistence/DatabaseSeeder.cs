using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesforceTest.Application.Interfaces;
using SalesforceTest.Domain.Entities;

namespace SalesforceTest.Infrastructure.Persistence;

public sealed class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AppDbContext context, IPasswordHasher passwordHasher, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.MigrateAsync(cancellationToken);

        await SeedUsersAsync(cancellationToken);
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        const string demoUsername = "demo";

        if (await _context.Users.AnyAsync(u => u.Username == demoUsername, cancellationToken))
            return;

        var demoUser = User.Create(
            username: demoUsername,
            email: "demo@salesforcetest.local",
            firstName: "Demo",
            lastName: "User",
            passwordHash: _passwordHasher.Hash("demo")
        );

        _context.Users.Add(demoUser);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Demo user seeded successfully.");
    }
}

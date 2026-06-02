using SalesforceTest.Domain.Common;

namespace SalesforceTest.Domain.Entities;

public sealed class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public static User Create(string username, string email, string firstName, string lastName, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new User
        {
            Username = username.ToLowerInvariant(),
            Email = email.ToLowerInvariant(),
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash
        };
    }

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public void Deactivate() => IsActive = false;
}

using SalesforceTest.Api.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace SalesforceTest.Api.Services;

internal sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword) => BC.HashPassword(plainPassword);
    public bool Verify(string plainPassword, string hash) => BC.Verify(plainPassword, hash);
}

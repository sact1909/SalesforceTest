using SalesforceTest.Api.Entities;

namespace SalesforceTest.Api.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}

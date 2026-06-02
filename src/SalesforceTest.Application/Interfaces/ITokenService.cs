using SalesforceTest.Domain.Entities;

namespace SalesforceTest.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}

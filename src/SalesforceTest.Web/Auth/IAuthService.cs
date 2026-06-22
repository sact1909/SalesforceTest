using SalesforceTest.Web.Models;

namespace SalesforceTest.Web.Auth;

public interface IAuthService
{
    Task<(bool Success, string? Error)> LoginAsync(LoginModel model, CancellationToken cancellationToken = default);
    Task LogoutAsync();
}

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SalesforceTest.Web.Models;
using System.Security.Claims;

namespace SalesforceTest.Web.Auth;

public sealed class AppAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    private bool _initialized = false;

    public AppAuthStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_initialized)
        {
            _initialized = true;
            try
            {
                var result = await _sessionStorage.GetAsync<AuthUser>("auth_user");
                if (result.Success && result.Value is not null && result.Value.ExpiresAt > DateTime.UtcNow)
                    _currentUser = BuildPrincipal(result.Value);
            }
            catch
            {
                // Storage not available yet (SSR) — stay anonymous
            }
        }

        return new AuthenticationState(_currentUser);
    }

    public void NotifyUserAuthenticated(AuthUser user)
    {
        _initialized = true;
        _currentUser = BuildPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void NotifyUserLoggedOut()
    {
        _initialized = true;
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    private static ClaimsPrincipal BuildPrincipal(AuthUser user) =>
        new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FullName", user.FullName),
            new Claim("ExpiresAt", user.ExpiresAt.ToString("o")),
        ], authenticationType: "jwt"));
}

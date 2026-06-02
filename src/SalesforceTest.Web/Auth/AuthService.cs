using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SalesforceTest.Web.Models;
using SalesforceTest.Web.Services;
using System.Text.Json;

namespace SalesforceTest.Web.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly AppAuthStateProvider _authStateProvider;

    private const string SessionKey = "auth_user";

    public AuthService(
        IApiService apiService,
        ProtectedSessionStorage sessionStorage,
        AppAuthStateProvider authStateProvider)
    {
        _apiService = apiService;
        _sessionStorage = sessionStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiService.PostAsync<object, JsonElement>(
                "api/auth/login",
                new { model.Username, model.Password },
                cancellationToken);

            var authUser = new AuthUser(
                Username: response.GetProperty("username").GetString()!,
                Email: response.GetProperty("email").GetString()!,
                FullName: response.GetProperty("fullName").GetString()!,
                Token: response.GetProperty("token").GetString()!,
                ExpiresAt: response.GetProperty("expiresAt").GetDateTime()
            );

            await _sessionStorage.SetAsync(SessionKey, authUser);
            _authStateProvider.NotifyUserAuthenticated(authUser);

            return (true, null);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            return (false, "Invalid username or password.");
        }
        catch
        {
            return (false, "Unable to connect to the server. Please try again.");
        }
    }

    public async Task LogoutAsync()
    {
        await _sessionStorage.DeleteAsync(SessionKey);
        _authStateProvider.NotifyUserLoggedOut();
    }

    public async Task<AuthUser?> GetCurrentUserAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<AuthUser>(SessionKey);
            if (!result.Success || result.Value is null)
                return null;

            if (result.Value.ExpiresAt <= DateTime.UtcNow)
            {
                await _sessionStorage.DeleteAsync(SessionKey);
                return null;
            }

            return result.Value;
        }
        catch
        {
            return null;
        }
    }
}

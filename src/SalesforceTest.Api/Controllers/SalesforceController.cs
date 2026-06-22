using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesforceTest.Application.Features.Salesforce;
using System.Security.Claims;

namespace SalesforceTest.Api.Controllers;

[Authorize]
public sealed class SalesforceController : ApiControllerBase
{
    private readonly GetAuthorizationUrlService _getAuthorizationUrlService;
    private readonly HandleOAuthCallbackService _handleOAuthCallbackService;
    private readonly GetSalesforceConnectionService _getSalesforceConnectionService;
    private readonly DisconnectSalesforceService _disconnectSalesforceService;
    private readonly IConfiguration _configuration;

    public SalesforceController(
        GetAuthorizationUrlService getAuthorizationUrlService,
        HandleOAuthCallbackService handleOAuthCallbackService,
        GetSalesforceConnectionService getSalesforceConnectionService,
        DisconnectSalesforceService disconnectSalesforceService,
        IConfiguration configuration)
    {
        _getAuthorizationUrlService = getAuthorizationUrlService;
        _handleOAuthCallbackService = handleOAuthCallbackService;
        _getSalesforceConnectionService = getSalesforceConnectionService;
        _disconnectSalesforceService = disconnectSalesforceService;
        _configuration = configuration;
    }

    [HttpGet("authorize")]
    public IActionResult GetAuthorizationUrl()
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = _getAuthorizationUrlService.Execute(userId.Value);
        return HandleResult(result);
    }

    [HttpGet("connection")]
    public async Task<IActionResult> GetConnection(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _getSalesforceConnectionService.ExecuteAsync(userId.Value, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("connection")]
    public async Task<IActionResult> Disconnect(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _disconnectSalesforceService.ExecuteAsync(userId.Value, cancellationToken);
        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state, CancellationToken cancellationToken)
    {
        var webBaseUrl = _configuration["AllowedOrigins:0"] ?? "https://localhost:7286";

        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            return Redirect($"{webBaseUrl}/sf-callback?sf_error=invalid_callback");

        var result = await _handleOAuthCallbackService.ExecuteAsync(code, state, cancellationToken);

        if (result.IsFailure)
            return Redirect($"{webBaseUrl}/sf-callback?sf_error={Uri.EscapeDataString(result.Error!)}");

        return Redirect($"{webBaseUrl}/sf-callback?sf_success=true");
    }

    private Guid? GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");

        return Guid.TryParse(sub, out var id) ? id : null;
    }
}

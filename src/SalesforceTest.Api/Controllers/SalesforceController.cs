using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesforceTest.Api.Features.Salesforce;
using System.Security.Claims;

namespace SalesforceTest.Api.Controllers;

[Authorize]
public sealed class SalesforceController : ApiControllerBase
{
    private readonly GetAuthorizationUrlService _getAuthorizationUrlService;
    private readonly HandleOAuthCallbackService _handleOAuthCallbackService;
    private readonly GetSalesforceConnectionService _getSalesforceConnectionService;
    private readonly DisconnectSalesforceService _disconnectSalesforceService;
    private readonly GetAvailableObjectsService _getAvailableObjectsService;
    private readonly GetObjectRecordsService _getObjectRecordsService;
    private readonly RescanObjectsService _rescanObjectsService;
    private readonly RefreshObjectCountService _refreshObjectCountService;
    private readonly IConfiguration _configuration;

    public SalesforceController(
        GetAuthorizationUrlService getAuthorizationUrlService,
        HandleOAuthCallbackService handleOAuthCallbackService,
        GetSalesforceConnectionService getSalesforceConnectionService,
        DisconnectSalesforceService disconnectSalesforceService,
        GetAvailableObjectsService getAvailableObjectsService,
        GetObjectRecordsService getObjectRecordsService,
        RescanObjectsService rescanObjectsService,
        RefreshObjectCountService refreshObjectCountService,
        IConfiguration configuration)
    {
        _getAuthorizationUrlService = getAuthorizationUrlService;
        _handleOAuthCallbackService = handleOAuthCallbackService;
        _getSalesforceConnectionService = getSalesforceConnectionService;
        _disconnectSalesforceService = disconnectSalesforceService;
        _getAvailableObjectsService = getAvailableObjectsService;
        _getObjectRecordsService = getObjectRecordsService;
        _rescanObjectsService = rescanObjectsService;
        _refreshObjectCountService = refreshObjectCountService;
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

    [HttpGet("objects")]
    public async Task<IActionResult> GetAvailableObjects(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _getAvailableObjectsService.ExecuteAsync(userId.Value, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("objects/{objectApiName}/records")]
    public async Task<IActionResult> GetObjectRecords(string objectApiName, [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _getObjectRecordsService.ExecuteAsync(userId.Value, objectApiName, from, to, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("objects/rescan")]
    public async Task<IActionResult> RescanObjects(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _rescanObjectsService.ExecuteAsync(userId.Value, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("objects/{objectApiName}/refresh-count")]
    public async Task<IActionResult> RefreshObjectCount(string objectApiName, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _refreshObjectCountService.ExecuteAsync(userId.Value, objectApiName, cancellationToken);
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

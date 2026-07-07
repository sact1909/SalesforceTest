using Microsoft.AspNetCore.Mvc;
using SalesforceTest.Api.DTOs.Auth;
using SalesforceTest.Api.Features.Auth;

namespace SalesforceTest.Api.Controllers;

public sealed class AuthController : ApiControllerBase
{
    private readonly LoginService _loginService;

    public AuthController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginService.ExecuteAsync(request, cancellationToken);
        return HandleResult(result);
    }
}

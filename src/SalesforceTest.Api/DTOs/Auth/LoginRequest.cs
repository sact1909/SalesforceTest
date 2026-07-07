using System.ComponentModel.DataAnnotations;

namespace SalesforceTest.Api.DTOs.Auth;

public sealed record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

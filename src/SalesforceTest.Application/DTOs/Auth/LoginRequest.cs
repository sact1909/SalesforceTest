using System.ComponentModel.DataAnnotations;

namespace SalesforceTest.Application.DTOs.Auth;

public sealed record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

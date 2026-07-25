using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.Auth;

public sealed record LoginRequest : ARecord
{
    [Required]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
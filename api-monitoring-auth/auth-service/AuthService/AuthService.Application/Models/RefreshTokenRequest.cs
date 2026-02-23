using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models;

public sealed record RefreshTokenRequest
{
    [Required]
    public required string Token { get; init; }
}
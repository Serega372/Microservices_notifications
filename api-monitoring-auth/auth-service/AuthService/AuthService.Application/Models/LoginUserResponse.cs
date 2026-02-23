using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models;

public sealed record LoginUserResponse
{
    [Required]
    public required string Token { get; init; }
    
    [Required]
    public required string RefreshToken { get; init; }
}
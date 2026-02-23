using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models;

public sealed record LoginUserRequest
{
    [Required]
    public required string Username { get; init; }
    
    [Required]
    public required string Password { get; init; }
}
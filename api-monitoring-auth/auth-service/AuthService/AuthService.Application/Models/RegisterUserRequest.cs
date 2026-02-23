using System.ComponentModel.DataAnnotations;
using AuthService.Core.Enums;

namespace AuthService.Application.Models;

public sealed record RegisterUserRequest
{
    [Required]
    public required string Username { get; init; }
    
    [Required]
    public required string Password { get; init; }
    
    public UserRole? Role { get; init; }
}
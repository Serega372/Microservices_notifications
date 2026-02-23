using System.ComponentModel.DataAnnotations;
using AuthService.Core.Enums;

namespace AuthService.Core.Entities;

public sealed class User
{
    public long Id { get; init; }
    
    [Required]
    public required string Username { get; init; }
    
    [Required]
    public required string PasswordHash { get; init; }
    
    [Required]
    public required UserRole Role { get; init; }
}
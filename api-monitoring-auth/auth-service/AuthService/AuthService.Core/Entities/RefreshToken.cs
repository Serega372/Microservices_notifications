using System.ComponentModel.DataAnnotations;

namespace AuthService.Core.Entities;

public sealed class RefreshToken
{
    [Required]
    public required string Token { get; init; }
    
    public long Id { get; init; } 
    
    [Required]
    public required long UserId { get; init; }
    
    [Required]
    public required DateTime ExpiresAt { get; init; }
}
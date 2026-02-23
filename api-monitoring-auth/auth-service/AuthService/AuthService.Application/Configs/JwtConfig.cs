using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Configs;

public sealed class JwtConfig
{
    public string SecretKey { get; init; } = string.Empty;
    
    [Required]
    public required int ExpiresMinutes { get; init; }
    
    [Required]
    public required int RefreshTokenExpiresMinutes { get; init; }
}
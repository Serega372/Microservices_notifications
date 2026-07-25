using AuthService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthService.Core.Entities;

public sealed class UserEntity : AEntity
{
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    public required long RoleId { get; set; }

    [JsonIgnore]
    public RoleEntity? Role { get; set; }

    [JsonIgnore]
    public ICollection<AccessTokenEntity> AccessTokens { get; set; } = [];

    [JsonIgnore]
    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = [];
}
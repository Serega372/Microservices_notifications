using AuthService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthService.Core.Entities;

public sealed class AccessTokenEntity : AEntity
{
    [Required]
    public required Guid Token { get; set; }

    public DateTime? RevocationDate { get; set; }

    [Required]
    public required DateTime ExpirationDate { get; set; }

    [Required]
    public required long RefreshTokenId { get; set; }

    [Required]
    public required long UserId { get; set; }

    [JsonIgnore]
    public RefreshTokenEntity? RefreshToken { get; set; }

    [JsonIgnore]
    public UserEntity? User { get; set; }
}
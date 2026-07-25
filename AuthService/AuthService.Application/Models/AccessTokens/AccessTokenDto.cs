using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.AccessTokens;

public sealed record AccessTokenDto : ARecord
{
    [Required]
    public required long Id { get; init; }

    [Required]
    public required Guid Token { get; init; }

    public DateTime? RevocationDate { get; init; }

    [Required]
    public required DateTime ExpirationDate { get; init; }

    [Required]
    public required long UserId { get; init; }
}
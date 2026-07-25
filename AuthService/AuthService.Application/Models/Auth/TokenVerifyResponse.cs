using AuthService.Application.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Models.Auth;

public sealed record TokenVerifyResponse : ARecord
{
    [Required]
    public required long UserId { get; init; }

    [Required]
    public required DateTime ExpirationDate { get; init; }

    public IEnumerable<string> Permissions { get; init; } = [];
}
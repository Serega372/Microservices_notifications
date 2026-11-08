using System.ComponentModel.DataAnnotations;

namespace NotificationsService.Application.Models;

public sealed record TokenVerifyResponse
{
    [Required]
    public required long UserId { get; init; }

    [Required]
    public required Guid AccessToken { get; init; }

    [Required]
    public required DateTime ExpirationDate { get; init; }

    public IEnumerable<string> Permissions { get; init; } = [];
}
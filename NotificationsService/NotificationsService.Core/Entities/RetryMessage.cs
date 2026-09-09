using System.ComponentModel.DataAnnotations;

namespace NotificationsService.Core.Entities;

public class RetryMessage
{
    [Required]
    public required Guid Id { get; init; }

    [Required]
    public required string Type { get; init; }

    [Required]
    public required string Payload { get; init; }

    public int RetryCount { get; init; }
}
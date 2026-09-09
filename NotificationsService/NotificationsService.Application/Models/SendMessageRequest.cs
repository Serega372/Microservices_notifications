using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace NotificationsService.Application.Models;

public sealed record SendMessageRequest
{
    [Required]
    public required string Type { get; init; }

    [Required]
    public required JsonElement Payload { get; init; }

    public Guid? Id { get; init; }
}
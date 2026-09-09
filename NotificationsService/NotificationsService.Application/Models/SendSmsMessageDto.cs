using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Application.Models;

public sealed record SendSmsMessageDto
{
    [Required]
    [JsonPropertyName("recipients")]
    public required IEnumerable<string> Recipients { get; init; }

    [Required]
    [JsonPropertyName("sender")]
    public required string Sender { get; init; }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; init; }
}
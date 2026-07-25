using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmailService.Application.Models;

public sealed record EmailMessageResponse
{
    [Required]
    [JsonPropertyName("recipients")]
    public required IEnumerable<string> Recipients { get; init; }

    [Required]
    [JsonPropertyName("sender")]
    public required string Sender { get; init; }

    [Required]
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

    [JsonPropertyName("body")]
    public string? Body { get; init; }

    [JsonPropertyName("attachments")]
    public IEnumerable<string> Attachments { get; init; } = [];
}
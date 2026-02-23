using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PushService.Application.Models;

public sealed record PushMessageResponse
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
    
    [JsonPropertyName("body")]
    public string? Body { get; init; }
    
    [JsonPropertyName("title")]
    public string? Title { get; init; }
    
    [JsonPropertyName("attachments")]
    public IEnumerable<string> Attachments { get; init; } = [];
}
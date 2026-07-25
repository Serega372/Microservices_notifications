using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsGateway.Application.Models;

public sealed record SendPushMessageDto
{
    [Required]
    [JsonPropertyName("recipients")]
    public required IEnumerable<string> Recipients { get; init; }
    
    [Required]
    [JsonPropertyName("sender")]
    public required string Sender { get; init; }
    
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; init; }
    
    [JsonPropertyName("body")]
    public string? Body { get; init; }
    
    [JsonPropertyName("attachments")]
    public IEnumerable<string> Attachments { get; init; } = [];
}
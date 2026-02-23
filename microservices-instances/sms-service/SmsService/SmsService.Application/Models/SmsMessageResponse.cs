using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmsService.Application.Models;

public sealed record SmsMessageResponse
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
}
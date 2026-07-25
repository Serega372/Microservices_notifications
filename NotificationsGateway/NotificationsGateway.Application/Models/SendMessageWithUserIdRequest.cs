using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace NotificationsGateway.Application.Models;

public sealed record SendMessageWithUserIdRequest
{
    [Required]
    public required string Type { get; init; }
    
    [Required]
    public required JsonElement Payload { get; init; }
    
    [Required]
    public required long UserId { get; init; }
}
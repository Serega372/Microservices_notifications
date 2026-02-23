using System.ComponentModel.DataAnnotations;

namespace PushService.Core.Entities;

public sealed class PushMessage
{
    public Guid Id { get; set; }
    
    public string? Body { get; set; }
    
    public string? Title { get; init; }
    
    public IEnumerable<string> Attachments { get; init; } = [];
    
    [Required]
    public required IEnumerable<string> Recipients { get; set; }
    
    [Required]
    public required string Sender { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace SmsService.Core.Entities;

public sealed class SmsMessage
{
    public Guid Id { get; set; }
    
    public string? Body { get; set; }
    
    [Required]
    public required IEnumerable<string> Recipients { get; set; }
    
    [Required]
    public required string Sender { get; set; }
}
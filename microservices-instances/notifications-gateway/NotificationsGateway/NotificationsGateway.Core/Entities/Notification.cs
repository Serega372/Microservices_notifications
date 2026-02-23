using System.ComponentModel.DataAnnotations;

namespace NotificationsGateway.Core.Entities;

public sealed class Notification
{
    [Required]
    public required Guid Id { get; init; }
    
    public long UserId { get; init; }
    
    [Required]
    public required string Status { get; set; }
    
    [Required]
    public required string Type { get; init; }
    
    public DateTime CreationDate { get; init; }
    
    public DateTime? SendingDate { get; set; }
}
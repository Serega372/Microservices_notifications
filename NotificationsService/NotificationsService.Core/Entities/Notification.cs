using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class Notification : AEntity
{
    [Required]
    public required Guid SourceEventId { get; set; }

    [Required]
    public required string Payload { get; set; }

    [Required]
    public long UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public ICollection<NotificationDelivery> NotificationDeliveries { get; set; } = [];
}
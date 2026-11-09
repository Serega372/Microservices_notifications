using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class NotificationDelivery : AEntity
{
    public string Subject { get; set; } = string.Empty;

    [Required]
    public required string Body { get; set; }

    public string LastError { get; set; } = string.Empty;

    [Required]
    public required string Status { get; set; }

    public DateTime? DeliveredAt { get; set; }

    [Required]
    public required byte AttemptCount { get; set; }

    public string Description { get; set; } = string.Empty;

    [Required]
    public long UserDispatchChannelId { get; set; }

    [JsonIgnore]
    public UserDispatchChannel? UserDispatchChannel { get; set; }

    [Required]
    public long NotificationTemplateId { get; set; }

    [JsonIgnore]
    public NotificationTemplate? NotificationTemplate { get; set; }

    [Required]
    public long NotificationId { get; set; }

    [JsonIgnore]
    public Notification? Notification { get; set; }
}
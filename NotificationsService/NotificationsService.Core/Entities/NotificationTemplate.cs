using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class NotificationTemplate : AEntity
{
    [Required]
    public required string Name { get; set; }

    public string Subject { get; set; } = string.Empty;

    [Required]
    public required string Body { get; set; }

    [Required]
    public long NotificationRuleChannelId { get; set; }

    [JsonIgnore]
    public NotificationRuleChannel? NotificationRuleChannel { get; set; }

    [JsonIgnore]
    public ICollection<NotificationDelivery> NotificationDeliveries { get; set; } = [];
}
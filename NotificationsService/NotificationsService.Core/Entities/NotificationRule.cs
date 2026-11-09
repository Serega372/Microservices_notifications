using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class NotificationRule : AEntity
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string EventType { get; set; }

    [Required]
    public long NotificationCategoryId { get; set; }

    [JsonIgnore]
    public NotificationCategory? NotificationCategory { get; set; }

    [JsonIgnore]
    public ICollection<NotificationRuleChannel> NotificationRuleChannels { get; set; } = [];
}
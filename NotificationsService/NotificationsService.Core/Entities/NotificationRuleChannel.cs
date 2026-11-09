using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class NotificationRuleChannel : AEntity
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public long DispatchChannelId { get; set; }

    [JsonIgnore]
    public DispatchChannel? DispatchChannel { get; set; }

    [Required]
    public long NotificationRuleId { get; set; }

    [JsonIgnore]
    public NotificationRule? NotificationRule { get; set; }

    [JsonIgnore]
    public ICollection<NotificationTemplate> NotificationTemplates { get; set; } = [];
}
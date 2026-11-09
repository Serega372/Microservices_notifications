using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class NotificationCategory : AEntity
{
    [Required]
    public required string Name { get; set; }

    [JsonIgnore]
    public ICollection<NotificationRule> NotificationRules { get; set; } = [];

    [JsonIgnore]
    public ICollection<UserDeliveriesPreference> UserDeliveriesPreferences { get; set; } = [];
}
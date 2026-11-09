using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class DispatchChannel : AEntity
{
    [Required]
    public required string Name { get; set; }

    [JsonIgnore]
    public ICollection<NotificationRuleChannel> NotificationRuleChannels { get; set; } = [];

    [JsonIgnore]
    public ICollection<UserDeliveriesPreference> UserDeliveriesPreferences { get; set; } = [];

    [JsonIgnore]
    public ICollection<UserDispatchChannel> UserDispatchChannels { get; set; } = [];
}
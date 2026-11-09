using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class User : AEntity
{
    [Required]
    public required long ExternalId { get; set; }

    [Required]
    public required string Name { get; set; }

    [JsonIgnore]
    public ICollection<Notification> Notifications { get; set; } = [];

    [JsonIgnore]
    public ICollection<UserDeliveriesPreference> UserDeliveriesPreferences { get; set; } = [];

    [JsonIgnore]
    public ICollection<UserDispatchChannel> UserDispatchChannels { get; set; } = [];
}
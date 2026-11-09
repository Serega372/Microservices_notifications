using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class UserDeliveriesPreference : AEntity
{
    [Required]
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    [Required]
    public long NotificationCategoryId { get; set; }

    [JsonIgnore]
    public NotificationCategory? NotificationCategory { get; set; }

    [Required]
    public long DispatchChannelId { get; set; }

    [JsonIgnore]
    public DispatchChannel? DispatchChannel { get; set; }

    [Required]
    public long UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }
}
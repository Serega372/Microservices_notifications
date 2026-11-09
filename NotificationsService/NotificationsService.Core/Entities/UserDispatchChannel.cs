using NotificationsService.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationsService.Core.Entities;

public sealed class UserDispatchChannel : AEntity
{
    [Required]
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    [Required]
    public required string DestinationAddress { get; set; }

    public bool IsVerified { get; set; }

    public bool IsEnabled { get; set; }

    [Required]
    public long DispatchChannelId { get; set; }

    [JsonIgnore]
    public DispatchChannel? DispatchChannel { get; set; }

    [Required]
    public long UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public ICollection<NotificationDelivery> NotificationDeliveries { get; set; } = [];
}
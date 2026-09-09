using System.ComponentModel.DataAnnotations;

namespace NotificationsService.Application.Configs;

public sealed class KafkaProducerConfig
{
    [Required]
    public required string Host { get; init; }

    [Required]
    public required Dictionary<string, string> Topics { get; init; }
}
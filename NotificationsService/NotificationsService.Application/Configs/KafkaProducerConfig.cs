namespace NotificationsService.Application.Configs;

public sealed class KafkaProducerConfig
{
    public required string Host { get; init; }

    public required Dictionary<string, string> Topics { get; init; }
}
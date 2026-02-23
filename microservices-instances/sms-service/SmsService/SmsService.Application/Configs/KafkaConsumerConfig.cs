namespace SmsService.Application.Configs;

public sealed class KafkaConsumerConfig
{
    public required string Host { get; set; }
    
    public required string Topic { get; set; }
    
    public required string GroupId { get; set; }
}
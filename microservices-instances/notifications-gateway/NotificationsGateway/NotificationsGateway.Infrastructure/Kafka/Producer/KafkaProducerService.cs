using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationsGateway.Application.Configs;
using NotificationsGateway.Application.Interfaces;

namespace NotificationsGateway.Infrastructure.Kafka.Producer;

public sealed class KafkaProducerService<TMessage> : IKafkaProducerService<TMessage>
{
    private readonly ILogger<KafkaProducerService<TMessage>> _logger;
    
    private readonly IProducer<string?, byte[]> _producer;

    private readonly string _topic;

    public KafkaProducerService(
        ILogger<KafkaProducerService<TMessage>> logger,
        IOptions<KafkaProducerConfig> config)
    {
        _logger = logger;
        var kafkaConfig = config.Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaConfig.Host,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 1,
            MessageTimeoutMs = 5000
        };
        
        _producer = new ProducerBuilder<string?, byte[]>(producerConfig).Build();
        _topic = kafkaConfig.Topics[typeof(TMessage).Name];
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var compressedMessage = await CompressMessageAsync(message);
            var result = await _producer.ProduceAsync(_topic, new Message<string?, byte[]>
            {
                Value = compressedMessage
            }, cancellationToken);
            
            _logger.LogInformation($"Delivered message to partition {result.Partition} on topic {result.Topic}");
        }
        catch (ProduceException<string, byte[]> ex)
        {
            _logger.LogError($"Delivery failed: {ex.Error.Reason}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while producing message: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(15));
        _producer.Dispose();
    }

    private async Task<byte[]> CompressMessageAsync(TMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytesArray = Encoding.UTF8.GetBytes(json);

        using var msgStream = new MemoryStream();
        await using (var gzip = new GZipStream(msgStream, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            await gzip.WriteAsync(bytesArray);
        }

        return msgStream.ToArray();
    }
}
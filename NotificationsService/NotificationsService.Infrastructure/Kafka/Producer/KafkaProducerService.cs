using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationsService.Application.Configs;
using NotificationsService.Application.Interfaces;

namespace NotificationsService.Infrastructure.Kafka.Producer;

public sealed class KafkaProducerService<TMessage> : IKafkaProducerService<TMessage>
{
    private readonly ILogger<KafkaProducerService<TMessage>> _logger;

    private readonly IProducer<string?, TMessage> _producer;

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

        _producer = new ProducerBuilder<string?, TMessage>(producerConfig).Build();
        _topic = kafkaConfig.Topics[typeof(TMessage).Name];

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _producer.ProduceAsync(_topic, new Message<string?, TMessage>
            {
                Value = message
            }, cancellationToken);

            _logger.LogInformation($"Delivered message to partition {result.Partition} on topic {result.Topic}");
        }
        catch (ProduceException<string, TMessage> ex)
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
}
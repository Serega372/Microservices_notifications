using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushService.Application.Configs;
using PushService.Application.Interfaces;

namespace PushService.Infrastructure.Kafka.Consumer;

public sealed class KafkaConsumerService<TMessage> : BackgroundService
{
    private readonly ILogger<KafkaConsumerService<TMessage>> _logger;

    private readonly IConsumer<Ignore, byte[]> _consumer;
    
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly string _topic;

    public KafkaConsumerService(
        IOptions<KafkaConsumerConfig> kafkaConfig,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<KafkaConsumerService<TMessage>> logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        var config = kafkaConfig.Value;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config.Host,
            GroupId = config.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };
        
        _consumer = new ConsumerBuilder<Ignore, byte[]>(consumerConfig).Build();
        _topic = config.Topic;
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    _logger.LogInformation($"received message in bytes from [topic={
                        consumeResult.Topic}, partition={consumeResult.Partition}]");
                    
                    var decompressedMessage = await DecompressMessageAsync(consumeResult.Message.Value);
                    _logger.LogInformation($"decompressed message successfully in {GetType().Name}");
                    
                    using var scope = _serviceScopeFactory.CreateScope();
                    var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();

                    var updatedNotificationId = await messageHandler.HandleAsync(decompressedMessage);
                    if (updatedNotificationId is null)
                    {
                        _logger.LogWarning($"Message processing may went wrong in {GetType().Name}");
                    }

                    _consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogCritical("Kafka consumer is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Unexpected error processing Kafka message: {ex.Message}");
                    throw;
                }
            }
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Error executing Kafka message");
        }
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Closing Kafka consumer");
        _consumer.Close();
        _consumer.Dispose();
        return base.StopAsync(stoppingToken);
    }
    
    private async Task<TMessage> DecompressMessageAsync(byte[] compressedMessage)
    {
        using var compressedStream = new MemoryStream(compressedMessage);
        await using var gzip = new GZipStream(compressedStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        await gzip.CopyToAsync(decompressedStream);
        var json = Encoding.UTF8.GetString(decompressedStream.ToArray());

        return JsonSerializer.Deserialize<TMessage>(json);
    }
}
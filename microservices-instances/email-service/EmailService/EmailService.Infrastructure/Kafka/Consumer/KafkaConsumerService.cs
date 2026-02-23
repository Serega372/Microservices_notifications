using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EmailService.Application.Configs;
using EmailService.Application.Interfaces;

namespace EmailService.Infrastructure.Kafka.Consumer;

public sealed class KafkaConsumerService<EmailMessageResponse> : BackgroundService
{
    private readonly ILogger<KafkaConsumerService<EmailMessageResponse>> _logger;

    private readonly IConsumer<Ignore, byte[]> _consumer;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly string _topic;

    public KafkaConsumerService(
        IOptions<KafkaConsumerConfig> kafkaConfig,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<KafkaConsumerService<EmailMessageResponse>> logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        var config = kafkaConfig.Value;
        _topic = config.Topic;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config.Host,
            GroupId = config.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            AllowAutoCreateTopics = true
        };

        _consumer = new ConsumerBuilder<Ignore, byte[]>(consumerConfig).Build();
        _consumer.Subscribe(_topic);

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult is null || consumeResult.IsPartitionEOF) continue;

                var compressedMessage = consumeResult.Message.Value;
                if (compressedMessage is null) continue;

                var message = await DecompressMessageAsync(compressedMessage);
                if (message is null) continue;

                await ProcessMessageAsync(message, cancellationToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Kafka message");
            }
        }
    }

    private async Task ProcessMessageAsync(EmailMessageResponse message, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler<EmailMessageResponse>>();

        try
        {
            var result = await messageHandler.HandleAsync(message);
            if (result is null)
            {
                throw new Exception("Message handling failed");
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

    private async Task<EmailMessageResponse> DecompressMessageAsync(byte[] compressedMessage)
    {
        using var compressedStream = new MemoryStream(compressedMessage);
        await using var gzip = new GZipStream(compressedStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        await gzip.CopyToAsync(decompressedStream);
        var json = Encoding.UTF8.GetString(decompressedStream.ToArray());

        return JsonSerializer.Deserialize<EmailMessageResponse>(json);
    }
}
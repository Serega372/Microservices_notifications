using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationsService.Application.Interfaces;
using NotificationsService.Application.Models;
using NotificationsService.Core.Consts;
using NotificationsService.Core.Entities;
using System.Text.Json;

namespace NotificationsService.Application.Services;

public sealed class RetryMessagesService : BackgroundService
{
    private readonly ILogger<RetryMessagesService> _logger;

    private readonly IServiceScopeFactory _scopeFactory;

    public RetryMessagesService(
        ILogger<RetryMessagesService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _scopeFactory = serviceScopeFactory;
        _logger = logger;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var retryMessagesRepository = scope.ServiceProvider.GetRequiredService<IRetryMessagesRepository>();
                var notificationsRepository = scope.ServiceProvider.GetRequiredService<INotificationsRepository>();
                var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();

                var batch =
                    await retryMessagesRepository.GetLockBatchAsync(cancellationToken);
                if (batch.Item2 is null || !batch.Item1.Any())
                {
                    _logger.LogInformation($"No retry messages found to retry in {GetType().Name}");
                    await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                    continue;
                }

                var transaction = batch.Item2;
                foreach (var message in batch.Item1)
                {
                    try
                    {
                        if (message.RetryCount == 2)
                        {
                            var notification = new Notification
                            {
                                Id = message.Id,
                                Status = MessageStatus.Error,
                                Type = message.Type
                            };

                            await retryMessagesRepository.DeleteAsync(message.Id, transaction, cancellationToken);
                            await notificationsRepository.UpdateAsync(notification, transaction, cancellationToken);
                        }

                        await ProcessMessageAsync(message, notificationsService, cancellationToken);
                        var deletedFromRetryMessageId =
                            await retryMessagesRepository.DeleteAsync(message.Id, transaction, cancellationToken);

                        if (deletedFromRetryMessageId is null)
                        {
                            _logger.LogWarning($"Failed delete retry message with id {message.Id} in {GetType().Name}");
                            continue;
                        }

                        _logger.LogInformation($"Successfully deleted retry message with id {message.Id} in {GetType().Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error occured while try to send retry message with [id={message.Id}] in {GetType().Name}: {ex.Message}");

                        await retryMessagesRepository.UpdateRetryCountAsync(message.Id, transaction, cancellationToken);
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while background retrying in {GetType().Name}: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(
        RetryMessage message,
        INotificationsService notificationsService,
        CancellationToken cancellationToken = default)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            Id = message.Id,
            Type = message.Type,
            Payload = JsonSerializer.Deserialize<JsonElement>(message.Payload),
        };

        await notificationsService.ProcessRetryMessageAsync(sendMessageRequest, cancellationToken);
    }
}
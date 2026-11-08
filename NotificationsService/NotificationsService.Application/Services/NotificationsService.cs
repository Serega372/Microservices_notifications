using Microsoft.Extensions.Logging;
using NotificationsService.Application.Interfaces;
using NotificationsService.Application.Models;
using NotificationsService.Core.Consts;
using NotificationsService.Core.Entities;
using System.Text.Json;

namespace NotificationsService.Application.Services;

public sealed class NotificationsService : INotificationsService
{
    private readonly ILogger<NotificationsService> _logger;

    private readonly INotificationsRepository _notificationsRepository;

    private readonly IRetryMessagesRepository _retryMessagesRepository;

    private readonly IKafkaProducerService<SendEmailMessageDto> _emailProducer;

    private readonly IKafkaProducerService<SendPushMessageDto> _pushProducer;

    private readonly IKafkaProducerService<SendSmsMessageDto> _smsProducer;

    public NotificationsService(
        ILogger<NotificationsService> logger,
        INotificationsRepository notificationsRepository,
        IRetryMessagesRepository retryMessagesRepository,
        IKafkaProducerService<SendEmailMessageDto> emailProducer,
        IKafkaProducerService<SendPushMessageDto> pushProducer,
        IKafkaProducerService<SendSmsMessageDto> smsProducer)
    {
        _logger = logger;
        _notificationsRepository = notificationsRepository;
        _retryMessagesRepository = retryMessagesRepository;
        _emailProducer = emailProducer;
        _pushProducer = pushProducer;
        _smsProducer = smsProducer;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<Guid?> ProcessMessageAsync(
        SendMessageWithUserIdRequest request,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Status = MessageStatus.Created,
            Type = request.Type,
            CreationDate = DateTime.UtcNow
        };

        var createdNotificationId = await _notificationsRepository.CreateAsync(notification, cancellationToken);
        if (createdNotificationId is null)
        {
            if (createdNotificationId is null)
            {
                _logger.LogWarning(
                    $"Notification [{notification.Type}] with [id={notification.Id}] create failed in {GetType().Name}");

                return null;
            }

            _logger.LogInformation(
                $"Notification [{notification.Type}] with [id={notification.Id}] updating succeeded in {GetType().Name}");
        }

        try
        {
            switch (request.Type)
            {
                case MessageType.Email:
                    var emailMessage = DeserializePayload<SendEmailMessageDto>(request.Payload);
                    emailMessage.Id = notification.Id;
                    await SendMessageToKafkaAsync(emailMessage, _emailProducer, cancellationToken);
                    break;

                case MessageType.Push:
                    var pushMessage = DeserializePayload<SendPushMessageDto>(request.Payload);
                    pushMessage.Id = notification.Id;
                    await SendMessageToKafkaAsync(pushMessage, _pushProducer, cancellationToken);
                    break;

                case MessageType.Sms:
                    var smsMessage = DeserializePayload<SendSmsMessageDto>(request.Payload);
                    smsMessage.Id = notification.Id;
                    await SendMessageToKafkaAsync(smsMessage, _smsProducer, cancellationToken);
                    break;
                default:
                    _logger.LogWarning(
                        $"Unknown type of [notification={request.Type}] was requested to send in {GetType().Name}");
                    return null;
            }

            notification.SendingDate = DateTime.UtcNow;
            notification.Status = MessageStatus.Sent;
            var updatedNotificationId = await _notificationsRepository.UpdateAsync(
                notification, cancellationToken: cancellationToken);
            if (updatedNotificationId is null)
            {
                _logger.LogWarning(
                    $"Notification [{notification.Type}] with [id={notification.Id}] updating failed in {GetType().Name}");

                return null;
            }

            _logger.LogInformation(
                $"Notification [{notification.Type}] with [id={notification.Id}] updating succeeded in {GetType().Name}");

            return notification.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while process message in {GetType().Name}: {ex.Message}");
            var updatedNotificationId = await MakeAsRetryAsync(notification, cancellationToken);
            if (updatedNotificationId is null)
            {
                throw;
            }

            var createdMessageToRetryId =
                await SaveRetryMessageAsync(request, updatedNotificationId.Value, cancellationToken);

            throw;
        }
    }

    public async Task ProcessRetryMessageAsync(
        SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            switch (request.Type)
            {
                case MessageType.Email:
                    var emailMessage = DeserializePayload<SendEmailMessageDto>(request.Payload);
                    emailMessage.Id = request.Id.Value;
                    await SendMessageToKafkaAsync(emailMessage, _emailProducer, cancellationToken);
                    break;

                case MessageType.Push:
                    var pushMessage = DeserializePayload<SendPushMessageDto>(request.Payload);
                    pushMessage.Id = request.Id.Value;
                    await SendMessageToKafkaAsync(pushMessage, _pushProducer, cancellationToken);
                    break;

                case MessageType.Sms:
                    var smsMessage = DeserializePayload<SendSmsMessageDto>(request.Payload);
                    smsMessage.Id = request.Id.Value;
                    await SendMessageToKafkaAsync(smsMessage, _smsProducer, cancellationToken);
                    break;
            }

            var notification = new Notification
            {
                Id = request.Id.Value,
                SendingDate = DateTime.UtcNow,
                Status = MessageStatus.Sent,
                Type = request.Type
            };

            var updatedNotificationId = await _notificationsRepository.UpdateAsync(
                notification, cancellationToken: cancellationToken);
            if (updatedNotificationId is null)
            {
                _logger.LogWarning($"Notification updated failed in {GetType().Name}");
                return;
            }

            _logger.LogInformation($"Notification successfully updated in {GetType().Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    private async Task SendMessageToKafkaAsync<TMessage>(
        TMessage message,
        IKafkaProducerService<TMessage> producer,
        CancellationToken cancellationToken = default)
    {
        await producer.ProduceAsync(message, cancellationToken);
        _logger.LogInformation($"Message with [request name={typeof(TMessage).Name}] sent to kafka in {GetType().Name}");
    }

    private TOut DeserializePayload<TOut>(
        JsonElement payload)
    {
        var deserializedMessage = payload.Deserialize<TOut>() ?? throw new InvalidOperationException();
        return deserializedMessage;
    }

    private async Task<Guid?> MakeAsRetryAsync(
        Notification notification,
        CancellationToken cancellationToken = default)
    {
        notification.Status = MessageStatus.Retry;
        notification.SendingDate = null;
        var updatedNotificationId = await _notificationsRepository.UpdateAsync(
            notification, cancellationToken: cancellationToken);
        if (updatedNotificationId is null)
        {
            _logger.LogWarning(
                $"Notification [{notification.Type}] with [id={notification.Id}] updating failed in {GetType().Name}");

            return null;
        }

        _logger.LogInformation(
            $"Notification [{notification.Type}] with [id={notification.Id}] updating succeeded in {GetType().Name}");

        return updatedNotificationId;
    }

    private async Task<Guid?> SaveRetryMessageAsync(
        SendMessageWithUserIdRequest request,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var retryMessage = new RetryMessage
        {
            Id = id,
            Payload = request.Payload.GetRawText(),
            Type = request.Type,
        };
        var createdMessageToRetry = await _retryMessagesRepository.CreateAsync(retryMessage, cancellationToken);

        return createdMessageToRetry;
    }
}
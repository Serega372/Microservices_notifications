using Microsoft.Extensions.Logging;
using PushService.Application.Interfaces;
using PushService.Application.Models;
using PushService.Core.Consts;
using PushService.Core.Entities;
using PushService.Core.Interfaces;

namespace PushService.Application.Handlers;

public sealed class PushMessageHandler : IMessageHandler<PushMessageResponse>
{
    private readonly ILogger<PushMessageHandler> _logger;
    
    private readonly IPushMessagesRepository _pushMessagesRepository;
    
    private readonly INotificationsRepository _notificationsRepository;

    public PushMessageHandler(
        IPushMessagesRepository pushMessagesRepository,
        INotificationsRepository notificationsRepository,
        ILogger<PushMessageHandler> logger)
    {
        _logger = logger;
        _pushMessagesRepository = pushMessagesRepository;
        _notificationsRepository = notificationsRepository;
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    public async Task<Guid?> HandleAsync(PushMessageResponse message)
    {
        try
        {
            var random = new Random();
            var randomValue = random.Next(1, 10);
            if (randomValue >= 8)
            {
                throw new Exception($"Error while send push message to recipient(s) in {GetType().Name}");
            }

            _logger.LogInformation($"Push message with [id={message.Id}] successfully sent to recipient(s) in {GetType().Name}");

            var pushMessage = new PushMessage
            {
                Id = message.Id,
                Body = message.Body,
                Title = message.Title,
                Sender = message.Sender,
                Recipients = message.Recipients,
                Attachments = message.Attachments
            };

            var updatePushMessageStatusTask = _notificationsRepository.UpdateStatusAsync(message.Id, MessageStatus.Success);
            var createPushMessageTask = _pushMessagesRepository.CreateAsync(pushMessage);

            await Task.WhenAll(updatePushMessageStatusTask, createPushMessageTask);

            var updatedMessageId = await updatePushMessageStatusTask;
            var createdMessageId = await createPushMessageTask;
            if (updatedMessageId is null || createdMessageId is null)
            {
                _logger.LogWarning(
                    $"Message with [id={message.Id}] updating status or creating went wrong in {GetType().Name}");
                return null;
            }

            _logger.LogInformation($"Push message creation and updating status success in {GetType().Name}");

            await Task.Delay(randomValue * 10);

            return updatedMessageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while send push message to recipient(s) in {GetType().Name}");
            await _notificationsRepository.UpdateStatusAsync(message.Id, MessageStatus.Error);
            return null;
        }
    }
}
using Microsoft.Extensions.Logging;
using SmsService.Application.Interfaces;
using SmsService.Application.Models;
using SmsService.Core.Consts;
using SmsService.Core.Entities;
using SmsService.Core.Interfaces;

namespace SmsService.Application.Handlers;

public sealed class SmsMessageHandler : IMessageHandler<SmsMessageResponse>
{
    private readonly ILogger<SmsMessageHandler> _logger;
    
    private readonly ISmsMessagesRepository _smsMessagesRepository;
    
    private readonly INotificationsRepository _notificationsRepository;

    public SmsMessageHandler(
        ISmsMessagesRepository smsMessagesRepository,
        INotificationsRepository notificationsRepository,
        ILogger<SmsMessageHandler> logger)
    {
        _logger = logger;
        _smsMessagesRepository = smsMessagesRepository;
        _notificationsRepository = notificationsRepository;
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    public async Task<Guid?> HandleAsync(SmsMessageResponse message)
    {
        try
        {
            var random = new Random();
            var randomValue = random.Next(1, 10);
            if (randomValue >= 8)
            {
                throw new Exception("Error while send sms message to recipient(s) in");
            }

            _logger.LogInformation($"Sms message with [id={message.Id}] successfully sent to recipient(s) in {GetType().Name}");

            var smsMessage = new SmsMessage
            {
                Id = message.Id,
                Body = message.Body,
                Sender = message.Sender,
                Recipients = message.Recipients,
            };

            var updateSmsMessageStatusTask = _notificationsRepository.UpdateStatusAsync(message.Id, MessageStatus.Success);
            var createSmsMessageTask = _smsMessagesRepository.CreateAsync(smsMessage);

            await Task.WhenAll(updateSmsMessageStatusTask, createSmsMessageTask);

            var updatedMessageId = await updateSmsMessageStatusTask;
            var createdMessageId = await createSmsMessageTask;
            if (updatedMessageId is null || createdMessageId is null)
            {
                _logger.LogWarning(
                    $"Message with [id={message.Id}] updating status or creating went wrong in {GetType().Name}");
                return null;
            }

            _logger.LogInformation($"Sms message creation and updating status success in {GetType().Name}");

            await Task.Delay(randomValue * 10);

            return updatedMessageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while send sms message to recipient(s) in {GetType().Name}");
            await _notificationsRepository.UpdateStatusAsync(message.Id, MessageStatus.Error);
            return null;
        }
    }
}
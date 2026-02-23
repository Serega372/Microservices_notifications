using Microsoft.Extensions.Logging;
using EmailService.Application.Interfaces;
using EmailService.Application.Models;
using EmailService.Core.Consts;
using EmailService.Core.Entities;
using EmailService.Core.Interfaces;

namespace EmailService.Application.Handlers;

public sealed class EmailMessageHandler : IMessageHandler<EmailMessageResponse>
{
    private readonly ILogger<EmailMessageHandler> _logger;

    private readonly IEmailsRepository _emailsRepository;

    private readonly INotificationsRepository _notificationsRepository;

    public EmailMessageHandler(
        IEmailsRepository emailsRepository,
        INotificationsRepository notificationsRepository,
        ILogger<EmailMessageHandler> logger)
    {
        _logger = logger;
        _emailsRepository = emailsRepository;
        _notificationsRepository = notificationsRepository;
    }

    public async Task<Guid?> HandleAsync(EmailMessageResponse message)
    {
        try
        {
            var randomValue = Random.Shared.Next(1, 10);

            var emailMessage = new EmailMessage
            {
                Id = message.Id,
                Subject = message.Subject,
                Body = message.Body,
                Recipients = message.Recipients,
                Sender = message.Sender,
                Attachments = message.Attachments
            };

            var createEmailTask = _emailsRepository.CreateAsync(emailMessage);
            var updateStatusTask = _notificationsRepository.UpdateStatusAsync(message.Id, MessageStatus.Success);

            var createdEmailId = await createEmailTask;
            var updatedId = await updateStatusTask;

            if (updatedId is null || createdEmailId is null)
            {
                _logger.LogWarning($"Email with [id={message.Id}] updating status or creating went wrong in {GetType().Name}");
                return null;
            }

            _logger.LogInformation($"Email creation and updating status success in {GetType().Name}");

            await Task.Delay(randomValue * 10); // Симуляция отправки

            return updatedId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while send email to recipient(s) in {GetType().Name}");
            await _notificationsRepository.UpdateStatusAsync(message.Id, MessageStatus.Error);
            return null;
        }
    }
}
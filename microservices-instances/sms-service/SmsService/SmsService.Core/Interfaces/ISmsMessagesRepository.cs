using SmsService.Core.Entities;

namespace SmsService.Core.Interfaces;

public interface ISmsMessagesRepository
{
    Task<Guid?> CreateAsync(SmsMessage smsMessage, CancellationToken cancellationToken = default);
}
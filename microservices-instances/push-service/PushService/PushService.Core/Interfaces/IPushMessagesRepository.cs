using PushService.Core.Entities;

namespace PushService.Core.Interfaces;

public interface IPushMessagesRepository
{
    Task<Guid?> CreateAsync(PushMessage pushMessage, CancellationToken cancellationToken = default);
}
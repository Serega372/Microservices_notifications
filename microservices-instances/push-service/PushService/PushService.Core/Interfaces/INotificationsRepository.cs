namespace PushService.Core.Interfaces;

public interface INotificationsRepository
{
    Task<Guid?> UpdateStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
}
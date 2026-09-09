using NotificationsService.Application.Models;

namespace NotificationsService.Application.Interfaces;

public interface INotificationsService
{
    Task<Guid?> ProcessMessageAsync(SendMessageWithUserIdRequest request, CancellationToken cancellationToken = default);

    Task ProcessRetryMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default);
}
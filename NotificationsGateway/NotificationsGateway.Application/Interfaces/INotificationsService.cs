using NotificationsGateway.Application.Models;

namespace NotificationsGateway.Application.Interfaces;

public interface INotificationsService
{
    Task<Guid?> ProcessMessageAsync(SendMessageWithUserIdRequest request, CancellationToken cancellationToken = default);
    
    Task ProcessRetryMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default);
}
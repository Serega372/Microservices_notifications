using System.Data;
using NotificationsGateway.Core.Entities;

namespace NotificationsGateway.Core.Interfaces;

public interface INotificationsRepository
{
    Task<Guid?> CreateAsync(Notification notification, CancellationToken cancellationToken = default);
    
    Task<Guid?> UpdateAsync(Notification notification, IDbTransaction? transaction = null, 
        CancellationToken cancellationToken = default);
}
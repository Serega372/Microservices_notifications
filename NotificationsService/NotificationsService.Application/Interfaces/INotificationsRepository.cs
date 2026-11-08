using NotificationsService.Core.Entities;
using System.Data;

namespace NotificationsService.Application.Interfaces;

public interface INotificationsRepository
{
    Task<Guid?> CreateAsync(Notification notification, CancellationToken cancellationToken = default);

    Task<Guid?> UpdateAsync(Notification notification, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);
}
using System.Data;
using NotificationsGateway.Core.Entities;

namespace NotificationsGateway.Core.Interfaces;

public interface IRetryMessagesRepository
{
    Task<Guid?> CreateAsync(RetryMessage message, CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<RetryMessage>, IDbTransaction?)> GetLockBatchAsync(CancellationToken cancellationToken = default);
    
    Task<Guid?> DeleteAsync(Guid id, IDbTransaction transaction, CancellationToken cancellationToken = default);
    
    Task<Guid?> UpdateRetryCountAsync(Guid id, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
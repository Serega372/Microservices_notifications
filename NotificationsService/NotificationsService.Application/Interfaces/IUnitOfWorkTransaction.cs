namespace NotificationsService.Application.Interfaces;

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
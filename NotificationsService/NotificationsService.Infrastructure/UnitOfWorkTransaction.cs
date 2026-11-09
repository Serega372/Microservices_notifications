using Microsoft.EntityFrameworkCore.Storage;
using NotificationsService.Application.Interfaces;

namespace NotificationsService.Infrastructure;

public sealed class UnitOfWorkTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
{
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => 
        transaction.CommitAsync(cancellationToken);

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
        transaction.RollbackAsync(cancellationToken);

    public ValueTask DisposeAsync() =>
        transaction.DisposeAsync();
}
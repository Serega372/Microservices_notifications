using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using PushService.Core.Consts;
using PushService.Core.Interfaces;
using PushService.Infrastructure.DataSources;

namespace PushService.Infrastructure.Repositories;

public sealed class NotificationsRepository : INotificationsRepository
{
    private readonly ILogger<NotificationsRepository> _logger;
    
    private readonly NpgsqlDataSource _dataSource;

    public NotificationsRepository(
        NotificationsDataSource dataSource,
        ILogger<NotificationsRepository> logger)
    {
        _logger = logger;
        _dataSource = dataSource.DataSource;
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<Guid?> UpdateStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "UPDATE notifications_tbl " +
            "SET status = @status, receiving_date = @receiving_date " +
            "WHERE id = @id " +
            "RETURNING id;";
        
        var parameters = new DynamicParameters();
        parameters.Add("@status", status);
        parameters.Add("@id", id);
        parameters.Add("@receiving_date", status == MessageStatus.Success ? DateTime.UtcNow : null);
        
        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var updatedNotificationId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (updatedNotificationId is null)
            {
                _logger.LogWarning($"Notification status update failed [id={id}] in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"Notification successfully updated with [id={updatedNotificationId}] in {GetType().Name}");
            return updatedNotificationId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occured while updating notification status with [id={
                id}, status={status}] in {GetType().Name}");
            throw;
        }
    }
}
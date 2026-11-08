using Dapper;
using Microsoft.Extensions.Logging;
using NotificationsService.Application.Interfaces;
using NotificationsService.Core.Entities;
using Npgsql;
using System.Data;

namespace NotificationsService.Infrastructure.Repositories;

public sealed class NotificationsRepository : INotificationsRepository
{
    private readonly ILogger<NotificationsRepository> _logger;

    private readonly NpgsqlDataSource _dataSource;

    public NotificationsRepository(
        NpgsqlDataSource dataSource,
        ILogger<NotificationsRepository> logger)
    {
        _logger = logger;
        _dataSource = dataSource;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<Guid?> CreateAsync(
        Notification notification,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "INSERT INTO notifications_tbl " +
            "(id, user_id, status, type, creation_date, sending_date) " +
            "VALUES " +
            "(@id, @user_id, @status, @type, @creation_date, @sending_date) " +
            "RETURNING id;";

        var parameters = new DynamicParameters();
        parameters.Add("@id", notification.Id);
        parameters.Add("@user_id", notification.UserId);
        parameters.Add("@status", notification.Status);
        parameters.Add("@type", notification.Type);
        parameters.Add("@creation_date", notification.CreationDate);
        parameters.Add("@sending_date", notification.SendingDate);

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var createdNotificationId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (createdNotificationId is null)
            {
                _logger.LogWarning($"Notification creation failed [notification={notification}] in {GetType().Name}");
                return null;
            }

            _logger.LogInformation($"Notification successfully created with [id={createdNotificationId}] in {GetType().Name}");
            return createdNotificationId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Guid?> UpdateAsync(
        Notification notification,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "UPDATE notifications_tbl " +
            "SET status = @status, sending_date = @sending_date " +
            "WHERE id = @id " +
            "RETURNING id;";

        var parameters = new DynamicParameters();
        parameters.Add("@status", notification.Status);
        parameters.Add("@sending_date", notification.SendingDate);
        parameters.Add("@id", notification.Id);

        var command = new CommandDefinition(
            sql,
            parameters,
            transaction,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var updatedNotificationId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (updatedNotificationId is null)
            {
                _logger.LogWarning($"Notification update failed [notification={notification}] in {GetType().Name}");
                return null;
            }

            _logger.LogInformation($"Notification successfully updated with [id={updatedNotificationId}] in {GetType().Name}");
            return updatedNotificationId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
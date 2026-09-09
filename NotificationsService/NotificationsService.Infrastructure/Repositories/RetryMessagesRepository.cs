using Dapper;
using Microsoft.Extensions.Logging;
using NotificationsService.Core.Entities;
using NotificationsService.Core.Interfaces;
using Npgsql;
using System.Data;

namespace NotificationsService.Infrastructure.Repositories;

public sealed class RetryMessagesRepository : IRetryMessagesRepository
{
    private readonly ILogger<RetryMessagesRepository> _logger;

    private readonly NpgsqlDataSource _dataSource;

    public RetryMessagesRepository(
        NpgsqlDataSource dataSource,
        ILogger<RetryMessagesRepository> logger)
    {
        _logger = logger;
        _dataSource = dataSource;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<Guid?> CreateAsync(
        RetryMessage message,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "INSERT INTO retry_messages_tbl " +
            "(id, type, payload) " +
            "VALUES " +
            "(@id, @type, CAST(@payload AS jsonb)) " +
            "RETURNING id;";

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", message.Id);
            parameters.Add("@type", message.Type);
            parameters.Add("@payload", message.Payload);

            var command = new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cancellationToken);

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var createdMessageToRetry = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (createdMessageToRetry is null)
            {
                _logger.LogWarning($"Message to retry creation failed [message={message}] in {GetType().Name}");
                return null;
            }

            _logger.LogInformation($"Message to retry successfully created with [id={createdMessageToRetry}] in {GetType().Name}");
            return createdMessageToRetry;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<(IEnumerable<RetryMessage>, IDbTransaction?)> GetLockBatchAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "SELECT " +
            "id AS Id, " +
            "type AS Type, " +
            "payload AS Payload, " +
            "retry_count AS RetryCount " +
            "FROM public.retry_messages_tbl " +
            "WHERE retry_count <= 2 " +
            "FOR UPDATE SKIP LOCKED " +
            "LIMIT 10 ";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        var command = new CommandDefinition(
            sql,
            transaction: transaction,
            cancellationToken: cancellationToken);

        try
        {
            var messagesBatch = await connection.QueryAsync<RetryMessage>(command);
            if (!messagesBatch.Any())
            {
                _logger.LogInformation($"No messages to retry in {GetType().Name}");
                return ([], null);
            }

            _logger.LogInformation($"Found [count={messagesBatch.Count()}] messages to retry in {GetType().Name}");

            return (messagesBatch, transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while getting retry messages in {GetType().Name}: {ex.Message}");

            throw;
        }
    }

    public async Task<Guid?> DeleteAsync(
        Guid id,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "DELETE FROM retry_messages_tbl " +
            "WHERE id = @id " +
            "RETURNING id;";

        var parameters = new DynamicParameters();
        parameters.Add("@id", id);

        var command = new CommandDefinition(
            sql,
            parameters,
            transaction,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var deletedMessageId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (deletedMessageId is null)
            {
                _logger.LogWarning($"No retry message to delete [id={id}] in {GetType().Name}");

                return null;
            }

            _logger.LogInformation($"Message to retry successfully deleted with [id={id}] in {GetType().Name}");

            return deletedMessageId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while deleting retry message [id={id}] in {GetType().Name}: {ex.Message}");

            throw;
        }
    }

    public async Task<Guid?> UpdateRetryCountAsync(
        Guid id,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "UPDATE retry_messages_tbl " +
            "SET retry_count = retry_count + 1 " +
            "WHERE id = @id " +
            "RETURNING id;";

        var parameters = new DynamicParameters();
        parameters.Add("@id", id);

        var command = new CommandDefinition(
            sql,
            parameters,
            transaction,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var updatedRetryMessageId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (updatedRetryMessageId is null)
            {
                _logger.LogWarning($"No retry message to update with [id={id}] in {GetType().Name}");

                return null;
            }

            _logger.LogInformation($"Message to retry successfully updated with [id={id}] in {GetType().Name}");

            return updatedRetryMessageId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while updating retry message [id={id}] in {GetType().Name}: {ex.Message}");

            throw;
        }
    }
}
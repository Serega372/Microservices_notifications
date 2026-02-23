using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using PushService.Core.Entities;
using PushService.Core.Interfaces;
using PushService.Infrastructure.DataSources;

namespace PushService.Infrastructure.Repositories;

public sealed class PushMessagesRepository : IPushMessagesRepository
{
    private readonly ILogger<PushMessagesRepository> _logger;
    
    private readonly NpgsqlDataSource _dataSource;

    public PushMessagesRepository(
        MessagesDataSource dataSource,
        ILogger<PushMessagesRepository> logger)
    {
        _logger = logger;
        _dataSource = dataSource.DataSource;
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    public async Task<Guid?> CreateAsync(
        PushMessage pushMessage,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "INSERT INTO public.push_tbl " +
            "(id, body, title, sender, recipients, attachments) " +
            "VALUES " +
            "(@id, @body, @title, @sender, @recipients, @attachments) " +
            "ON CONFLICT (id) " +
            "DO UPDATE SET " +
                "body = EXCLUDED.body, " +
                "title = EXCLUDED.title, " +
                "sender = EXCLUDED.sender, " +
                "recipients = EXCLUDED.recipients, " +
                "attachments = EXCLUDED.attachments " +
            "RETURNING id;";
        
        var parameters = new DynamicParameters();
        parameters.Add("@id", pushMessage.Id);
        parameters.Add("@body", pushMessage.Body);
        parameters.Add("@title", pushMessage.Title);
        parameters.Add("@sender", pushMessage.Sender);
        parameters.Add("@recipients", pushMessage.Recipients);
        parameters.Add("@attachments", pushMessage.Attachments);
        
        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var createdPushMessageId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (createdPushMessageId is null)
            {
                _logger.LogWarning($"Push message creation failed [message={pushMessage}] in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"Push message successfully created with [id={createdPushMessageId}] in {GetType().Name}");
            return createdPushMessageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
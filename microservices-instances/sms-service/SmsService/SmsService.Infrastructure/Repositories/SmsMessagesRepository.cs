using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using SmsService.Core.Entities;
using SmsService.Core.Interfaces;
using SmsService.Infrastructure.DataSources;

namespace SmsService.Infrastructure.Repositories;

public sealed class SmsMessagesRepository : ISmsMessagesRepository
{
    private readonly ILogger<SmsMessagesRepository> _logger;
    
    private readonly NpgsqlDataSource _dataSource;

    public SmsMessagesRepository(
        MessagesDataSource dataSource,
        ILogger<SmsMessagesRepository> logger)
    {
        _logger = logger;
        _dataSource = dataSource.DataSource;
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    public async Task<Guid?> CreateAsync(
        SmsMessage smsMessage,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "INSERT INTO public.sms_tbl " +
            "(id, body, sender, recipients) " +
            "VALUES " +
            "(@id, @body, @sender, @recipients) " +
            "ON CONFLICT (id) " +
            "DO UPDATE SET " +
                "body = EXCLUDED.body, " +
                "sender = EXCLUDED.sender, " +
                "recipients = EXCLUDED.recipients " +
            "RETURNING id;";
        
        var parameters = new DynamicParameters();
        parameters.Add("@id", smsMessage.Id);
        parameters.Add("@body", smsMessage.Body);
        parameters.Add("@sender", smsMessage.Sender);
        parameters.Add("@recipients", smsMessage.Recipients);
        
        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var createdSmsMessageId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (createdSmsMessageId is null)
            {
                _logger.LogWarning($"Sms message creation failed [message={smsMessage}] in {GetType().Name}");
                return null;
            }
            
            _logger.LogInformation($"Sms message successfully created with [id={createdSmsMessageId}] in {GetType().Name}");
            return createdSmsMessageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
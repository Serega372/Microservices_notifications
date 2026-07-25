using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using EmailService.Core.Entities;
using EmailService.Core.Interfaces;
using EmailService.Infrastructure.DataSources;

namespace EmailService.Infrastructure.Repositories;

public sealed class EmailsRepository : IEmailsRepository
{
    private readonly ILogger<EmailsRepository> _logger;

    private readonly NpgsqlDataSource _dataSource;

    public EmailsRepository(
        MessagesDataSource dataSource,
        ILogger<EmailsRepository> logger)
    {
        _logger = logger;
        _dataSource = dataSource.DataSource;

        _logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<Guid?> CreateAsync(
        EmailMessage emailMessage,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO emails_tbl (id, subject, body, sender, recipients, attachments)
            VALUES (@Id, @Subject, @Body, @Sender, @Recipients, @Attachments)
            RETURNING id;
            """;

        var parameters = new
        {
            emailMessage.Id,
            emailMessage.Subject,
            emailMessage.Body,
            emailMessage.Sender,
            Recipients = emailMessage.Recipients.ToArray(), // TEXT[]
            Attachments = emailMessage.Attachments.ToArray() // TEXT[]
        };

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var createdEmailId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
            if (createdEmailId is null)
            {
                _logger.LogWarning($"Email creation failed [message={emailMessage}] in {GetType().Name}");
                return null;
            }

            _logger.LogInformation($"Email successfully created with [id={createdEmailId}] in {GetType().Name}");
            return createdEmailId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
using Npgsql;

namespace SmsService.Infrastructure.DataSources;

public sealed class MessagesDataSource(NpgsqlDataSource dataSource)
{
    public NpgsqlDataSource DataSource { get; } = dataSource;
}
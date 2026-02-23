using Npgsql;

namespace PushService.Infrastructure.DataSources;

public sealed class MessagesDataSource(NpgsqlDataSource dataSource)
{
    public NpgsqlDataSource DataSource { get; } = dataSource;
}
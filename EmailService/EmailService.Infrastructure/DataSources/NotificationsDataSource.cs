using Npgsql;

namespace EmailService.Infrastructure.DataSources;

public sealed class NotificationsDataSource(NpgsqlDataSource dataSource)
{
    public NpgsqlDataSource DataSource { get; } = dataSource;
}
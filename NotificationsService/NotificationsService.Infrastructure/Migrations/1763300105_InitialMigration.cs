using FluentMigrator;

namespace NotificationsService.Infrastructure.Migrations;

[Migration(1763300105, "initial-migration")]
public class InitialMigration : Migration
{
    public override void Down()
    {
        Delete.Table("notifications_tbl");
        Delete.Table("retry_messages_tbl");
    }

    public override void Up()
    {
        Create.Table("notifications_tbl")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("status").AsString().NotNullable()
            .WithColumn("type").AsString().NotNullable()
            .WithColumn("creation_date").AsDateTime().NotNullable()
            .WithColumn("sending_date").AsDateTime().Nullable()
            .WithColumn("receiving_date").AsDateTime().Nullable();

        Create.Table("retry_messages_tbl")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("type").AsString().NotNullable()
            .WithColumn("payload").AsCustom("jsonb").NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("last_retry_date").AsDateTime().Nullable();
    }
}
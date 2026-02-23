using FluentMigrator;

namespace PushService.Infrastructure.Migrations;

[Migration(1763300106, "initial-migration")]
public class InitialMigration : Migration
{
    public override void Down()
    {
        Delete.Table("push_tbl");
    }
    
    public override void Up()
    {
        Create.Table("push_tbl")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("body").AsString().Nullable()
            .WithColumn("title").AsString().Nullable()
            .WithColumn("sender").AsString().NotNullable()
            .WithColumn("recipients").AsCustom("TEXT[]")
            .WithColumn("attachments").AsCustom("TEXT[]");
    }
}
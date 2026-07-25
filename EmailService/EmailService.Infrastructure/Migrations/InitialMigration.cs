using FluentMigrator;

namespace EmailService.Infrastructure.Migrations;

[Migration(1763300107, "initial-migration")]
public class InitialMigration : Migration
{
    public override void Down()
    {
        Delete.Table("emails_tbl");
    }

    public override void Up()
    {
        Create.Table("emails_tbl")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("subject").AsString().Nullable()
            .WithColumn("body").AsString().Nullable()
            .WithColumn("sender").AsString().NotNullable()
            .WithColumn("recipients").AsCustom("TEXT[]")
            .WithColumn("attachments").AsCustom("TEXT[]");
    }
}
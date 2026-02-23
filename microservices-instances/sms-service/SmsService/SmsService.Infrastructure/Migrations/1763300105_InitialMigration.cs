using FluentMigrator;

namespace SmsService.Infrastructure.Migrations;

[Migration(1763300105, "initial-migration")]
public class InitialMigration : Migration
{
    public override void Down()
    {
        Delete.Table("sms_tbl");
    }
    
    public override void Up()
    {
        Create.Table("sms_tbl")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("body").AsString().Nullable()
            .WithColumn("sender").AsString().NotNullable()
            .WithColumn("recipients").AsCustom("TEXT[]");
    }
}
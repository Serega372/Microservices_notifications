using FluentMigrator;

namespace ApiGateway.Infrastructure.Migrations;

[Migration(17627833331, "initial-migration")]
public class InitialMigration : Migration
{
    public override void Down()
    {
        Delete.Table("users_tbl");
        Delete.Table("refresh_tokens_tbl");
    }

    public override void Up()
    {
        Create.Table("users_tbl")
            .WithColumn("id").AsCustom("BIGSERIAL").NotNullable().PrimaryKey()
            .WithColumn("username").AsString().NotNullable().Unique()
            .WithColumn("password_hash").AsString().NotNullable()
            .WithColumn("role").AsInt32().NotNullable();

        Create.Table("refresh_tokens_tbl")
            .WithColumn("id").AsCustom("BIGSERIAL").NotNullable().PrimaryKey()
            .WithColumn("token").AsString().NotNullable()
            .WithColumn("user_id").AsInt64().Unique().NotNullable()
            .WithColumn("expires_at").AsDateTime().NotNullable();
        
        CreateAdminEntity();
    }
    
    private void CreateAdminEntity()
    {
        Insert.IntoTable("users_tbl")
            .InSchema("public")
            .Row(new
            {
                id = 0,
                username = "admin",
                password_hash = "$2a$11$9yz7sHlJH/4r4903TsktvOiFFLyeV3ftMVBFuSdTIHVA53ZzWD.Bm",
                role = 0
            });
    }
}
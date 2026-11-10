using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NotificationsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dispatch_channels_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dispatch_channels_tbl", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_categories_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_categories_tbl", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    external_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users_tbl", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_rules_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    notification_category_id = table.Column<long>(type: "bigint", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_rules_tbl", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_rules_tbl_notification_categories_tbl_notifica",
                        column: x => x.notification_category_id,
                        principalTable: "notification_categories_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    source_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications_tbl", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_tbl_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_deliveries_preferences_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    notification_category_id = table.Column<long>(type: "bigint", nullable: false),
                    dispatch_channel_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_deliveries_preferences_tbl", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_deliveries_preferences_tbl_dispatch_channels_tbl_dispa",
                        column: x => x.dispatch_channel_id,
                        principalTable: "dispatch_channels_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_deliveries_preferences_tbl_notification_categories_tbl",
                        column: x => x.notification_category_id,
                        principalTable: "notification_categories_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_deliveries_preferences_tbl_users_tbl_user_id",
                        column: x => x.user_id,
                        principalTable: "users_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_dispatch_channels_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    destination_address = table.Column<string>(type: "text", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    dispatch_channel_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_dispatch_channels_tbl", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_dispatch_channels_tbl_dispatch_channels_tbl_dispatch_c",
                        column: x => x.dispatch_channel_id,
                        principalTable: "dispatch_channels_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_dispatch_channels_tbl_users_tbl_user_id",
                        column: x => x.user_id,
                        principalTable: "users_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_rule_channels_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    dispatch_channel_id = table.Column<long>(type: "bigint", nullable: false),
                    notification_rule_id = table.Column<long>(type: "bigint", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_rule_channels_tbl", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_rule_channels_tbl_dispatch_channels_tbl_dispat",
                        column: x => x.dispatch_channel_id,
                        principalTable: "dispatch_channels_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notification_rule_channels_tbl_notification_rules_notificat",
                        column: x => x.notification_rule_id,
                        principalTable: "notification_rules_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_templates_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    notification_rule_channel_id = table.Column<long>(type: "bigint", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_templates_tbl", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_templates_tbl_notification_rule_channels_tbl_n",
                        column: x => x.notification_rule_channel_id,
                        principalTable: "notification_rule_channels_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_deliveries_tbl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    subject = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    last_error = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    delivered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    attempt_count = table.Column<byte>(type: "smallint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    user_dispatch_channel_id = table.Column<long>(type: "bigint", nullable: false),
                    notification_template_id = table.Column<long>(type: "bigint", nullable: false),
                    notification_id = table.Column<long>(type: "bigint", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_deliveries_tbl", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_deliveries_tbl_notification_templates_notifica",
                        column: x => x.notification_template_id,
                        principalTable: "notification_templates_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notification_deliveries_tbl_notifications_tbl_notification_",
                        column: x => x.notification_id,
                        principalTable: "notifications_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notification_deliveries_tbl_user_dispatch_channels_user_dis",
                        column: x => x.user_dispatch_channel_id,
                        principalTable: "user_dispatch_channels_tbl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_dispatch_channels_tbl_is_deleted",
                table: "dispatch_channels_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_dispatch_channels_tbl_name",
                table: "dispatch_channels_tbl",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_categories_tbl_is_deleted",
                table: "notification_categories_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_notification_categories_tbl_name",
                table: "notification_categories_tbl",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_deliveries_tbl_is_deleted",
                table: "notification_deliveries_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_notification_deliveries_tbl_notification_id",
                table: "notification_deliveries_tbl",
                column: "notification_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_deliveries_tbl_notification_template_id",
                table: "notification_deliveries_tbl",
                column: "notification_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_deliveries_tbl_user_dispatch_channel_id",
                table: "notification_deliveries_tbl",
                column: "user_dispatch_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_rule_channels_tbl_dispatch_channel_id",
                table: "notification_rule_channels_tbl",
                column: "dispatch_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_rule_channels_tbl_is_deleted",
                table: "notification_rule_channels_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_notification_rule_channels_tbl_notification_rule_id",
                table: "notification_rule_channels_tbl",
                column: "notification_rule_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_rules_tbl_event_type",
                table: "notification_rules_tbl",
                column: "event_type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_rules_tbl_is_deleted",
                table: "notification_rules_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_notification_rules_tbl_notification_category_id",
                table: "notification_rules_tbl",
                column: "notification_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_templates_tbl_is_deleted",
                table: "notification_templates_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_notification_templates_tbl_notification_rule_channel_id",
                table: "notification_templates_tbl",
                column: "notification_rule_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_tbl_is_deleted",
                table: "notifications_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_tbl_user_id",
                table: "notifications_tbl",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_deliveries_preferences_tbl_dispatch_channel_id",
                table: "user_deliveries_preferences_tbl",
                column: "dispatch_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_deliveries_preferences_tbl_is_deleted",
                table: "user_deliveries_preferences_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_deliveries_preferences_tbl_notification_category_id",
                table: "user_deliveries_preferences_tbl",
                column: "notification_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_deliveries_preferences_tbl_user_id",
                table: "user_deliveries_preferences_tbl",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_dispatch_channels_tbl_dispatch_channel_id",
                table: "user_dispatch_channels_tbl",
                column: "dispatch_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_dispatch_channels_tbl_is_deleted",
                table: "user_dispatch_channels_tbl",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_dispatch_channels_tbl_user_id",
                table: "user_dispatch_channels_tbl",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_tbl_external_id",
                table: "users_tbl",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_tbl_is_deleted",
                table: "users_tbl",
                column: "is_deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_deliveries_tbl");

            migrationBuilder.DropTable(
                name: "user_deliveries_preferences_tbl");

            migrationBuilder.DropTable(
                name: "notification_templates_tbl");

            migrationBuilder.DropTable(
                name: "notifications_tbl");

            migrationBuilder.DropTable(
                name: "user_dispatch_channels_tbl");

            migrationBuilder.DropTable(
                name: "notification_rule_channels_tbl");

            migrationBuilder.DropTable(
                name: "users_tbl");

            migrationBuilder.DropTable(
                name: "dispatch_channels_tbl");

            migrationBuilder.DropTable(
                name: "notification_rules_tbl");

            migrationBuilder.DropTable(
                name: "notification_categories_tbl");
        }
    }
}

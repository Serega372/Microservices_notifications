using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissions_tbl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions_tbl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles_tbl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles_tbl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles_permissions_tbl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles_permissions_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roles_permissions_tbl_permissions_tbl_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roles_permissions_tbl_roles_tbl_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users_tbl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_tbl_roles_tbl_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens_tbl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Token = table.Column<Guid>(type: "uuid", nullable: false),
                    RevocationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_tbl_users_tbl_UserId",
                        column: x => x.UserId,
                        principalTable: "users_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "access_tokens_tbl",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Token = table.Column<Guid>(type: "uuid", nullable: false),
                    RevocationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshTokenId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_tokens_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_access_tokens_tbl_refresh_tokens_tbl_RefreshTokenId",
                        column: x => x.RefreshTokenId,
                        principalTable: "refresh_tokens_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_access_tokens_tbl_users_tbl_UserId",
                        column: x => x.UserId,
                        principalTable: "users_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_access_tokens_tbl_IsDeleted",
                table: "access_tokens_tbl",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_access_tokens_tbl_RefreshTokenId",
                table: "access_tokens_tbl",
                column: "RefreshTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_access_tokens_tbl_UserId",
                table: "access_tokens_tbl",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_tbl_IsDeleted",
                table: "permissions_tbl",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_tbl_IsDeleted",
                table: "refresh_tokens_tbl",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_tbl_UserId",
                table: "refresh_tokens_tbl",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_permissions_tbl_IsDeleted",
                table: "roles_permissions_tbl",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_roles_permissions_tbl_PermissionId",
                table: "roles_permissions_tbl",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_permissions_tbl_RoleId",
                table: "roles_permissions_tbl",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_tbl_IsDeleted",
                table: "roles_tbl",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_users_tbl_IsDeleted",
                table: "users_tbl",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_users_tbl_RoleId",
                table: "users_tbl",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_tokens_tbl");

            migrationBuilder.DropTable(
                name: "roles_permissions_tbl");

            migrationBuilder.DropTable(
                name: "refresh_tokens_tbl");

            migrationBuilder.DropTable(
                name: "permissions_tbl");

            migrationBuilder.DropTable(
                name: "users_tbl");

            migrationBuilder.DropTable(
                name: "roles_tbl");
        }
    }
}

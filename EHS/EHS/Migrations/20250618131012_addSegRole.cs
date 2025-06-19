using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class addSegRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seg_role",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seg_role", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seg_role",
                schema: "ehs");
        }
    }
}

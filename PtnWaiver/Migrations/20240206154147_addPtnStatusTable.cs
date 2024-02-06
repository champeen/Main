using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addPtnStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PtnStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Default = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: false),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtnStatus", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PtnStatus");
        }
    }
}

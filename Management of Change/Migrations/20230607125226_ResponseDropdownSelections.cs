using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class ResponseDropdownSelections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Order",
                table: "ChangeType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Order",
                table: "ChangeStep",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Order",
                table: "ChangeLevel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResponseDropdownSelections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseDropdownSelections", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResponseDropdownSelections");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ChangeType");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ChangeStep");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ChangeLevel");
        }
    }
}

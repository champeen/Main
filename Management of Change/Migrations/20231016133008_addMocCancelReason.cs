using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addMocCancelReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Cancel_Date",
                table: "ChangeRequest",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cancel_Reason",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cancel_Username",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancel_Date",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Cancel_Reason",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Cancel_Username",
                table: "ChangeRequest");
        }
    }
}

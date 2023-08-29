using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addImplementationAndCloseoutFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Task",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "Closeout_Date",
                table: "ChangeRequest",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Closeout_Username",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Implementation_Approval_Date",
                table: "ChangeRequest",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Implementation_Username",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Closeout_Date",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Closeout_Username",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Implementation_Approval_Date",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Implementation_Username",
                table: "ChangeRequest");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Task",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}

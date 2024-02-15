using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addNavigationFieldPtn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Waiver",
                newName: "WaiverNumber");

            migrationBuilder.AddColumn<DateTime>(
                name: "CorrectiveActionDueDate",
                table: "Waiver",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateClosed",
                table: "Waiver",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PorProject",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductProcess",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RevisionNumber",
                table: "Waiver",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectiveActionDueDate",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "DateClosed",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "PorProject",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "ProductProcess",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "RevisionNumber",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Waiver");

            migrationBuilder.RenameColumn(
                name: "WaiverNumber",
                table: "Waiver",
                newName: "Name");
        }
    }
}

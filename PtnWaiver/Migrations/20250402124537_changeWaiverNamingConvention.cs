using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class changeWaiverNamingConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Areas",
                table: "Waiver",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateSequence",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalIdMes",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WaiverSequence",
                table: "Waiver",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Areas",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "DateSequence",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "ExternalIdMes",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "WaiverSequence",
                table: "Waiver");
        }
    }
}

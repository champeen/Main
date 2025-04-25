using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Classification",
                table: "ChangeRequest",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Ramp_Up_Approval_Date",
                table: "ChangeRequest",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ramp_Up_Stage1_Complete",
                table: "ChangeRequest",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ramp_Up_Stage2_Complete",
                table: "ChangeRequest",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ramp_Up_Stage3_Complete",
                table: "ChangeRequest",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ramp_Up_Username",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Classification",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Ramp_Up_Approval_Date",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Ramp_Up_Stage1_Complete",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Ramp_Up_Stage2_Complete",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Ramp_Up_Stage3_Complete",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Ramp_Up_Username",
                table: "ChangeRequest");
        }
    }
}

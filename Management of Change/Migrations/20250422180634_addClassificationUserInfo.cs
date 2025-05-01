using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addClassificationUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClassificationApprovalDate",
                table: "ChangeRequest",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassificationApprovalUser",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassificationApprovalUserFullName",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassificationApprovalDate",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ClassificationApprovalUser",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ClassificationApprovalUserFullName",
                table: "ChangeRequest");
        }
    }
}

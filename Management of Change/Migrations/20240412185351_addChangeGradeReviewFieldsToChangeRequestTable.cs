using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addChangeGradeReviewFieldsToChangeRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeGradeApprovalDate",
                table: "ChangeRequest",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeGradeApprovalUser",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeGradeApprovalUserFullName",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeGradeApprovalDate",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ChangeGradeApprovalUser",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ChangeGradeApprovalUserFullName",
                table: "ChangeRequest");
        }
    }
}

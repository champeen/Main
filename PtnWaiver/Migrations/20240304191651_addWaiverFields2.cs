using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addWaiverFields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedByAdminDate",
                table: "Waiver",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByAdminlUser",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByAdminlUserFullName",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedByDate",
                table: "Waiver",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBylUser",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBylUserFullName",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedForAdminApprovalDate",
                table: "Waiver",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedForAdminApprovalUser",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedForAdminApprovalUserFullName",
                table: "Waiver",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByAdminDate",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "ApprovedByAdminlUser",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "ApprovedByAdminlUserFullName",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "CompletedByDate",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "CompletedBylUser",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "CompletedBylUserFullName",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "SubmittedForAdminApprovalDate",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "SubmittedForAdminApprovalUser",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "SubmittedForAdminApprovalUserFullName",
                table: "Waiver");
        }
    }
}

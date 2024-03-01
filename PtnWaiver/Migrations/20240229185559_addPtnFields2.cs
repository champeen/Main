using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addPtnFields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedByAdminDate",
                table: "PTN",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByAdminlUser",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByAdminlUserFullName",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedByDate",
                table: "PTN",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBylUser",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBylUserFullName",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedForAdminApprovalDate",
                table: "PTN",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedForAdminApprovalUser",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedForAdminApprovalUserFullName",
                table: "PTN",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByAdminDate",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "ApprovedByAdminlUser",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "ApprovedByAdminlUserFullName",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "CompletedByDate",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "CompletedBylUser",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "CompletedBylUserFullName",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "SubmittedForAdminApprovalDate",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "SubmittedForAdminApprovalUser",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "SubmittedForAdminApprovalUserFullName",
                table: "PTN");
        }
    }
}

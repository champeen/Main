using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class changeGradeNameChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondaryApproverUsername",
                table: "ChangeArea",
                newName: "ChangeGradeSecondaryApproverUsername");

            migrationBuilder.RenameColumn(
                name: "SecondaryApproverTitle",
                table: "ChangeArea",
                newName: "ChangeGradeSecondaryApproverTitle");

            migrationBuilder.RenameColumn(
                name: "SecondaryApproverFullName",
                table: "ChangeArea",
                newName: "ChangeGradeSecondaryApproverFullName");

            migrationBuilder.RenameColumn(
                name: "SecondaryApproverEmail",
                table: "ChangeArea",
                newName: "ChangeGradeSecondaryApproverEmail");

            migrationBuilder.RenameColumn(
                name: "PrimaryApproverUsername",
                table: "ChangeArea",
                newName: "ChangeGradePrimaryApproverUsername");

            migrationBuilder.RenameColumn(
                name: "PrimaryApproverTitle",
                table: "ChangeArea",
                newName: "ChangeGradePrimaryApproverTitle");

            migrationBuilder.RenameColumn(
                name: "PrimaryApproverFullName",
                table: "ChangeArea",
                newName: "ChangeGradePrimaryApproverFullName");

            migrationBuilder.RenameColumn(
                name: "PrimaryApproverEmail",
                table: "ChangeArea",
                newName: "ChangeGradePrimaryApproverEmail");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeGradeRejectedDate",
                table: "ChangeRequest",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeGradeRejectedReason",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeGradeRejectedUser",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeGradeRejectedUserFullName",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeGradeRejectedDate",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ChangeGradeRejectedReason",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ChangeGradeRejectedUser",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ChangeGradeRejectedUserFullName",
                table: "ChangeRequest");

            migrationBuilder.RenameColumn(
                name: "ChangeGradeSecondaryApproverUsername",
                table: "ChangeArea",
                newName: "SecondaryApproverUsername");

            migrationBuilder.RenameColumn(
                name: "ChangeGradeSecondaryApproverTitle",
                table: "ChangeArea",
                newName: "SecondaryApproverTitle");

            migrationBuilder.RenameColumn(
                name: "ChangeGradeSecondaryApproverFullName",
                table: "ChangeArea",
                newName: "SecondaryApproverFullName");

            migrationBuilder.RenameColumn(
                name: "ChangeGradeSecondaryApproverEmail",
                table: "ChangeArea",
                newName: "SecondaryApproverEmail");

            migrationBuilder.RenameColumn(
                name: "ChangeGradePrimaryApproverUsername",
                table: "ChangeArea",
                newName: "PrimaryApproverUsername");

            migrationBuilder.RenameColumn(
                name: "ChangeGradePrimaryApproverTitle",
                table: "ChangeArea",
                newName: "PrimaryApproverTitle");

            migrationBuilder.RenameColumn(
                name: "ChangeGradePrimaryApproverFullName",
                table: "ChangeArea",
                newName: "PrimaryApproverFullName");

            migrationBuilder.RenameColumn(
                name: "ChangeGradePrimaryApproverEmail",
                table: "ChangeArea",
                newName: "PrimaryApproverEmail");
        }
    }
}

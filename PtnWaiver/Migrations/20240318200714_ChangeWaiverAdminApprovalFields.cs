using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWaiverAdminApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmittedForAdminApprovalUserFullName",
                table: "Waiver",
                newName: "SubmittedForApprovalUserFullName");

            migrationBuilder.RenameColumn(
                name: "SubmittedForAdminApprovalUser",
                table: "Waiver",
                newName: "SubmittedForApprovalUser");

            migrationBuilder.RenameColumn(
                name: "SubmittedForAdminApprovalDate",
                table: "Waiver",
                newName: "SubmittedForApprovalDate");

            migrationBuilder.RenameColumn(
                name: "RejectedByAdmin",
                table: "Waiver",
                newName: "RejectedByApprover");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminlUserFullName",
                table: "Waiver",
                newName: "ApprovedByUserFullName");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminlUser",
                table: "Waiver",
                newName: "ApprovedByUser");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminDate",
                table: "Waiver",
                newName: "ApprovedByDate");

            migrationBuilder.RenameColumn(
                name: "SubmittedForAdminApprovalUserFullName",
                table: "PTN",
                newName: "SubmittedForApprovalUserFullName");

            migrationBuilder.RenameColumn(
                name: "SubmittedForAdminApprovalUser",
                table: "PTN",
                newName: "SubmittedForApprovalUser");

            migrationBuilder.RenameColumn(
                name: "SubmittedForAdminApprovalDate",
                table: "PTN",
                newName: "SubmittedForApprovalDate");

            migrationBuilder.RenameColumn(
                name: "RejectedByAdmin",
                table: "PTN",
                newName: "RejectedByApprover");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminlUserFullName",
                table: "PTN",
                newName: "ApprovedByUserFullName");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminlUser",
                table: "PTN",
                newName: "ApprovedByUser");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminDate",
                table: "PTN",
                newName: "ApprovedByDate");

            migrationBuilder.AlterColumn<string>(
                name: "SecondaryApproverUsername",
                table: "Waiver",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SecondaryApproverFullName",
                table: "Waiver",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SecondaryApproverEmail",
                table: "Waiver",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryApproverUsername",
                table: "Waiver",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryApproverFullName",
                table: "Waiver",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryApproverEmail",
                table: "Waiver",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmittedForApprovalUserFullName",
                table: "Waiver",
                newName: "SubmittedForAdminApprovalUserFullName");

            migrationBuilder.RenameColumn(
                name: "SubmittedForApprovalUser",
                table: "Waiver",
                newName: "SubmittedForAdminApprovalUser");

            migrationBuilder.RenameColumn(
                name: "SubmittedForApprovalDate",
                table: "Waiver",
                newName: "SubmittedForAdminApprovalDate");

            migrationBuilder.RenameColumn(
                name: "RejectedByApprover",
                table: "Waiver",
                newName: "RejectedByAdmin");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserFullName",
                table: "Waiver",
                newName: "ApprovedByAdminlUserFullName");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUser",
                table: "Waiver",
                newName: "ApprovedByAdminlUser");

            migrationBuilder.RenameColumn(
                name: "ApprovedByDate",
                table: "Waiver",
                newName: "ApprovedByAdminDate");

            migrationBuilder.RenameColumn(
                name: "SubmittedForApprovalUserFullName",
                table: "PTN",
                newName: "SubmittedForAdminApprovalUserFullName");

            migrationBuilder.RenameColumn(
                name: "SubmittedForApprovalUser",
                table: "PTN",
                newName: "SubmittedForAdminApprovalUser");

            migrationBuilder.RenameColumn(
                name: "SubmittedForApprovalDate",
                table: "PTN",
                newName: "SubmittedForAdminApprovalDate");

            migrationBuilder.RenameColumn(
                name: "RejectedByApprover",
                table: "PTN",
                newName: "RejectedByAdmin");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserFullName",
                table: "PTN",
                newName: "ApprovedByAdminlUserFullName");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUser",
                table: "PTN",
                newName: "ApprovedByAdminlUser");

            migrationBuilder.RenameColumn(
                name: "ApprovedByDate",
                table: "PTN",
                newName: "ApprovedByAdminDate");

            migrationBuilder.AlterColumn<string>(
                name: "SecondaryApproverUsername",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondaryApproverFullName",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondaryApproverEmail",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryApproverUsername",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryApproverFullName",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryApproverEmail",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}

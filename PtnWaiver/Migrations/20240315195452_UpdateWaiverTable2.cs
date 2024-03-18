using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWaiverTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverEmail",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverFullName",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverTitle",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverUsername",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverEmail",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverFullName",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverTitle",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverUsername",
                table: "Waiver",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryApproverEmail",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverFullName",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverTitle",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverUsername",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverEmail",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverFullName",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverTitle",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverUsername",
                table: "Waiver");
        }
    }
}

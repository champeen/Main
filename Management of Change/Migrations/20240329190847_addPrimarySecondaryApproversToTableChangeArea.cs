using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addPrimarySecondaryApproversToTableChangeArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverEmail",
                table: "ChangeArea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverFullName",
                table: "ChangeArea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverTitle",
                table: "ChangeArea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverUsername",
                table: "ChangeArea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverEmail",
                table: "ChangeArea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverFullName",
                table: "ChangeArea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverTitle",
                table: "ChangeArea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverUsername",
                table: "ChangeArea",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryApproverEmail",
                table: "ChangeArea");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverFullName",
                table: "ChangeArea");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverTitle",
                table: "ChangeArea");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverUsername",
                table: "ChangeArea");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverEmail",
                table: "ChangeArea");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverFullName",
                table: "ChangeArea");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverTitle",
                table: "ChangeArea");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverUsername",
                table: "ChangeArea");
        }
    }
}

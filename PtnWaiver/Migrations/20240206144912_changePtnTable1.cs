using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class changePtnTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PTN",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PTN",
                newName: "TisNumber");

            migrationBuilder.AddColumn<string>(
                name: "DocId",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfLocation",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PtnPin",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Roadblocks",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectType",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocId",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "PdfLocation",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "PtnPin",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "Roadblocks",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "PTN");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "PTN",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "TisNumber",
                table: "PTN",
                newName: "Description");
        }
    }
}

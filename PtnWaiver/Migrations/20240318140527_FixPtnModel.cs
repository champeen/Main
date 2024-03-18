using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class FixPtnModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department_Area",
                table: "PTN");

            migrationBuilder.RenameColumn(
                name: "Group",
                table: "PTN",
                newName: "GroupApprover");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupApprover",
                table: "PTN",
                newName: "Group");

            migrationBuilder.AddColumn<string>(
                name: "Department_Area",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

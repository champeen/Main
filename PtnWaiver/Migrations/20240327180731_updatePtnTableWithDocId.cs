using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class updatePtnTableWithDocId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PtnPin",
                table: "PTN",
                newName: "OriginatingGroup");

            migrationBuilder.AddColumn<string>(
                name: "BouleSize",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginatorInitials",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginatorYear",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "PTN",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BouleSize",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "OriginatorInitials",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "OriginatorYear",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "PTN");

            migrationBuilder.RenameColumn(
                name: "OriginatingGroup",
                table: "PTN",
                newName: "PtnPin");
        }
    }
}

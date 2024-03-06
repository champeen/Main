using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addRejectedReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "PTN",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "PTN");
        }
    }
}

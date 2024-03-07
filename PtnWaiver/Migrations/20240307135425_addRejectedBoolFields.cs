using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addRejectedBoolFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RejectedBeforeSubmission",
                table: "Waiver",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RejectedByAdmin",
                table: "Waiver",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RejectedBeforeSubmission",
                table: "PTN",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RejectedByAdmin",
                table: "PTN",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedBeforeSubmission",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "RejectedByAdmin",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "RejectedBeforeSubmission",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "RejectedByAdmin",
                table: "PTN");
        }
    }
}

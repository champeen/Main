using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class add3FieldsToChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CMT_Number",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PTN_Number",
                table: "ChangeRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Waiver_Number",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CMT_Number",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "PTN_Number",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "Waiver_Number",
                table: "ChangeRequest");
        }
    }
}

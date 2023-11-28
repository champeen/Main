using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CMT_Number",
                table: "ChangeRequest");

            migrationBuilder.AddColumn<string>(
                name: "AssignedByUserEmail",
                table: "Task",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedByUserFullName",
                table: "Task",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserEmail",
                table: "Task",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserFullName",
                table: "Task",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledReason",
                table: "Task",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedByUserEmail",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "AssignedByUserFullName",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "AssignedToUserEmail",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "AssignedToUserFullName",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CancelledReason",
                table: "Task");

            migrationBuilder.AddColumn<string>(
                name: "CMT_Number",
                table: "ChangeRequest",
                type: "text",
                nullable: true);
        }
    }
}

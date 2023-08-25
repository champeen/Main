using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addApproverToAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approver",
                table: "Administrators",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver",
                table: "Administrators");
        }
    }
}

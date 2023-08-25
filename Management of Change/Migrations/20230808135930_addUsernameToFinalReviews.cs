using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addUsernameToFinalReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReviewerEmail",
                table: "ImplementationFinalApprovalResponse",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ImplementationFinalApprovalResponse",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "ImplementationFinalApprovalResponse");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewerEmail",
                table: "ImplementationFinalApprovalResponse",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addUsernameToReviewTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ReviewType",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "FinalReviewType",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "ReviewType");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "FinalReviewType");
        }
    }
}

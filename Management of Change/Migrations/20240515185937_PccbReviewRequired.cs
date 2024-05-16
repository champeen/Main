using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class PccbReviewRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewRequired",
                table: "ChangeLevel",
                newName: "PccbReviewRequired");

            migrationBuilder.AddColumn<bool>(
                name: "ChangeGradeReviewRequired",
                table: "ChangeLevel",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeGradeReviewRequired",
                table: "ChangeLevel");

            migrationBuilder.RenameColumn(
                name: "PccbReviewRequired",
                table: "ChangeLevel",
                newName: "ReviewRequired");
        }
    }
}

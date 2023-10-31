using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class updateAdditionalReviewerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewerEmail",
                table: "AdditionalImpactAssessmentReviewers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewerName",
                table: "AdditionalImpactAssessmentReviewers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewerEmail",
                table: "AdditionalImpactAssessmentReviewers");

            migrationBuilder.DropColumn(
                name: "ReviewerName",
                table: "AdditionalImpactAssessmentReviewers");
        }
    }
}

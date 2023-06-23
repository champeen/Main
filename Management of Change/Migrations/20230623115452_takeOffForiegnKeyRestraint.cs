using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class takeOffForiegnKeyRestraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChangeType_FinalReviewType_Index",
                table: "ImplementationFinalApprovalResponse");

            migrationBuilder.AlterColumn<string>(
                name: "FinalReviewType",
                table: "ImplementationFinalApprovalResponse",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ChangeType",
                table: "ImplementationFinalApprovalResponse",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FinalReviewType",
                table: "ImplementationFinalApprovalResponse",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ChangeType",
                table: "ImplementationFinalApprovalResponse",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeType_FinalReviewType_Index",
                table: "ImplementationFinalApprovalResponse",
                columns: new[] { "ChangeType", "FinalReviewType" },
                unique: true);
        }
    }
}

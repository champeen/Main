using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addWaiverForiegnKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaiverQuestionResponse_Waiver_WaiverId",
                table: "WaiverQuestionResponse");

            migrationBuilder.AlterColumn<int>(
                name: "WaiverId",
                table: "WaiverQuestionResponse",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WaiverQuestionResponse_Waiver_WaiverId",
                table: "WaiverQuestionResponse",
                column: "WaiverId",
                principalTable: "Waiver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaiverQuestionResponse_Waiver_WaiverId",
                table: "WaiverQuestionResponse");

            migrationBuilder.AlterColumn<int>(
                name: "WaiverId",
                table: "WaiverQuestionResponse",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_WaiverQuestionResponse_Waiver_WaiverId",
                table: "WaiverQuestionResponse",
                column: "WaiverId",
                principalTable: "Waiver",
                principalColumn: "Id");
        }
    }
}

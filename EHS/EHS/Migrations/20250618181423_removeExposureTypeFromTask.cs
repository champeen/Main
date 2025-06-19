using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class removeExposureTypeFromTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exposure_type",
                schema: "ehs",
                table: "task");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "exposure_type",
                schema: "ehs",
                table: "task",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

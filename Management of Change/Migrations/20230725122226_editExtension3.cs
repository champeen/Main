using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class editExtension3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AllowedAttachmentExtensions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AllowedAttachmentExtensions");
        }
    }
}

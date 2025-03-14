using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addWaiverMaterialDetailPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "AdditionalEmailNotificationsOfMaterialDetails",
                table: "Waiver",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialDetailNotes",
                table: "Waiver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isWaferingDepartment",
                table: "PTN",
                type: "boolean",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalEmailNotificationsOfMaterialDetails",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "MaterialDetailNotes",
                table: "Waiver");

            migrationBuilder.DropColumn(
                name: "isWaferingDepartment",
                table: "PTN");
        }
    }
}

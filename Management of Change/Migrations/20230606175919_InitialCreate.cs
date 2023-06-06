using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MOC_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Change_Owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location_Site = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title_Change_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope_of_the_Change = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Justification_of_the_Change = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Change_Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area_of_Change = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration_Date_Temporary = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Change_Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Request_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Proudct_Line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Change_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estimated_Completion_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Raw_Material_Component_Numbers_Impacted = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequest", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeRequest");
        }
    }
}

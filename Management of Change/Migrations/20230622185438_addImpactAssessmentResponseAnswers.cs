using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addImpactAssessmentResponseAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImpactAssessmentResponseAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailsOfActionNeeded = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreOrPostImplementation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionOwner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImpactAssessmentResponseId = table.Column<int>(type: "int", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpactAssessmentResponseAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpactAssessmentResponseAnswer_ImpactAssessmentResponse_ImpactAssessmentResponseId",
                        column: x => x.ImpactAssessmentResponseId,
                        principalTable: "ImpactAssessmentResponse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImpactAssessmentResponseAnswer_ImpactAssessmentResponseId",
                table: "ImpactAssessmentResponseAnswer",
                column: "ImpactAssessmentResponseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpactAssessmentResponseAnswer");
        }
    }
}

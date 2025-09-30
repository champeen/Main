using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class addWaiverQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PtnMandatoryApproval",
                table: "GroupApprovers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WaiverMandatoryApproval",
                table: "GroupApprovers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "WaiverQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    GroupApprover = table.Column<List<string>>(type: "text[]", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaiverQuestionResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WaiverId = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    GroupApprover = table.Column<List<string>>(type: "text[]", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    Response = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverQuestionResponse", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaiverQuestion");

            migrationBuilder.DropTable(
                name: "WaiverQuestionResponse");

            migrationBuilder.DropColumn(
                name: "PtnMandatoryApproval",
                table: "GroupApprovers");

            migrationBuilder.DropColumn(
                name: "WaiverMandatoryApproval",
                table: "GroupApprovers");
        }
    }
}

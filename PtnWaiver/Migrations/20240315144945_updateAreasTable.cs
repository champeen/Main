using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class updateAreasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.RenameColumn(
                name: "TisNumber",
                table: "PTN",
                newName: "SecondaryApproverTitle");

            migrationBuilder.RenameColumn(
                name: "Roadblocks",
                table: "PTN",
                newName: "PtrNumber");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "PTN",
                newName: "SecondaryApproverUsername");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department_Area",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverEmail",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverFullName",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverTitle",
                table: "PTN",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryApproverUsername",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverEmail",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryApproverFullName",
                table: "PTN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Department_Area",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverUsername = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverFullName = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverEmail = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverUsername = table.Column<string>(type: "text", nullable: false),
                    SecondaryApproverFullName = table.Column<string>(type: "text", nullable: false),
                    SecondaryApproverEmail = table.Column<string>(type: "text", nullable: false),
                    SecondaryApproverTitle = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Department_Area", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Department_Area");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "Department_Area",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverEmail",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverFullName",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverTitle",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "PrimaryApproverUsername",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverEmail",
                table: "PTN");

            migrationBuilder.DropColumn(
                name: "SecondaryApproverFullName",
                table: "PTN");

            migrationBuilder.RenameColumn(
                name: "SecondaryApproverUsername",
                table: "PTN",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "SecondaryApproverTitle",
                table: "PTN",
                newName: "TisNumber");

            migrationBuilder.RenameColumn(
                name: "PtrNumber",
                table: "PTN",
                newName: "Roadblocks");

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.Id);
                });
        }
    }
}

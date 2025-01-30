using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class addPccbStepTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingDate",
                table: "PCCB");

            migrationBuilder.DropColumn(
                name: "MeetingTime",
                table: "PCCB");

            migrationBuilder.AddColumn<string>(
                name: "InviteeList",
                table: "PCCB",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotificationList",
                table: "PCCB",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Step",
                table: "PCCB",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ListName = table.Column<string>(type: "text", nullable: false),
                    Emails = table.Column<List<string>>(type: "text[]", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PccbStep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PccbStep", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLists");

            migrationBuilder.DropTable(
                name: "PccbStep");

            migrationBuilder.DropColumn(
                name: "InviteeList",
                table: "PCCB");

            migrationBuilder.DropColumn(
                name: "NotificationList",
                table: "PCCB");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "PCCB");

            migrationBuilder.AddColumn<DateTime>(
                name: "MeetingDate",
                table: "PCCB",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MeetingTime",
                table: "PCCB",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}

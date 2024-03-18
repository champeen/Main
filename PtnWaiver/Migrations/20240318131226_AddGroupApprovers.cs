using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupApprovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Department_Area");

            migrationBuilder.CreateTable(
                name: "GroupApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Group = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverUsername = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverUsername = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverEmail = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_GroupApprovers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupApprovers");

            migrationBuilder.CreateTable(
                name: "Department_Area",
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
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverUsername = table.Column<string>(type: "text", nullable: false),
                    SecondaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverUsername = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department_Area", x => x.Id);
                });
        }
    }
}

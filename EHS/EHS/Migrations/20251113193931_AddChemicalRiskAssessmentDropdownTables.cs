using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class AddChemicalRiskAssessmentDropdownTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "area",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_area", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hazardous",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hazardous", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "physical_state",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_physical_state", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppe_eye",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ppe_eye", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppe_glove",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ppe_glove", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppe_respiratory",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ppe_respiratory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ppe_suit",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ppe_suit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "risk_eye_contact",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    risk_rating = table.Column<int>(type: "integer", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_eye_contact", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "risk_ingestion",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    risk_rating = table.Column<int>(type: "integer", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_ingestion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "risk_inhalation",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    risk_rating = table.Column<int>(type: "integer", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_inhalation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "risk_skin_contact",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    risk_rating = table.Column<int>(type: "integer", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_skin_contact", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "use",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<string>(type: "text", nullable: true),
                    display = table.Column<bool>(type: "boolean", nullable: false),
                    created_user = table.Column<string>(type: "text", nullable: false),
                    created_user_fullname = table.Column<string>(type: "text", nullable: true),
                    created_user_email = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_user = table.Column<string>(type: "text", nullable: true),
                    modified_user_fullname = table.Column<string>(type: "text", nullable: true),
                    modified_user_email = table.Column<string>(type: "text", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_user = table.Column<string>(type: "text", nullable: true),
                    deleted_user_fullname = table.Column<string>(type: "text", nullable: true),
                    deleted_user_email = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_use", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "area",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "hazardous",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "physical_state",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ppe_eye",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ppe_glove",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ppe_respiratory",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ppe_suit",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "risk_eye_contact",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "risk_ingestion",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "risk_inhalation",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "risk_skin_contact",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "use",
                schema: "ehs");
        }
    }
}

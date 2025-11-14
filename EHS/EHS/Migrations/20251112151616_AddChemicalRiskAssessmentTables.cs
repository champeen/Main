using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class AddChemicalRiskAssessmentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chemical_risk_assessment",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    location = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    area = table.Column<List<string>>(type: "text[]", nullable: false),
                    use = table.Column<List<string>>(type: "text[]", nullable: false),
                    state = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    chemical = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    nfpa = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    inhalation = table.Column<List<string>>(type: "text[]", nullable: false),
                    skin_contact = table.Column<List<string>>(type: "text[]", nullable: false),
                    eye_contact = table.Column<List<string>>(type: "text[]", nullable: false),
                    ingestion = table.Column<List<string>>(type: "text[]", nullable: false),
                    glove = table.Column<List<string>>(type: "text[]", nullable: false),
                    suit = table.Column<List<string>>(type: "text[]", nullable: false),
                    eyewear = table.Column<List<string>>(type: "text[]", nullable: false),
                    respiratory = table.Column<List<string>>(type: "text[]", nullable: false),
                    oels = table.Column<string>(type: "text", nullable: true),
                    emergency_response = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    risk_score = table.Column<int>(type: "integer", nullable: true),
                    person_performing_assessment_username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    person_performing_assessment_displayname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    date_conducted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_reviewed = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_chemical_risk_assessment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chemical_composition",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chemical_risk_assessment_id = table.Column<int>(type: "integer", nullable: false),
                    cas_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    chemical_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    concentration_low = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
                    concentration_high = table.Column<decimal>(type: "numeric(6,3)", nullable: true),
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
                    table.PrimaryKey("PK_chemical_composition", x => x.id);
                    table.ForeignKey(
                        name: "FK_chemical_composition_chemical_risk_assessment_chemical_risk~",
                        column: x => x.chemical_risk_assessment_id,
                        principalSchema: "ehs",
                        principalTable: "chemical_risk_assessment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_composition_chemicalriskassessmentid",
                schema: "ehs",
                table: "chemical_composition",
                column: "chemical_risk_assessment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chemical_composition",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "chemical_risk_assessment",
                schema: "ehs");
        }
    }
}

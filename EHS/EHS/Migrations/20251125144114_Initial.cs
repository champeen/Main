using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ehs");

            migrationBuilder.CreateTable(
                name: "acute_chronic",
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
                    table.PrimaryKey("PK_acute_chronic", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "agent",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exposure_type = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_agent", x => x.id);
                });

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
                name: "assessment_methods_used",
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
                    table.PrimaryKey("PK_assessment_methods_used", x => x.id);
                });

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
                    skin_absorption = table.Column<List<int>>(type: "integer[]", nullable: true),
                    skin_contact = table.Column<List<int>>(type: "integer[]", nullable: true),
                    eye_contact = table.Column<List<int>>(type: "integer[]", nullable: true),
                    respiratory = table.Column<List<int>>(type: "integer[]", nullable: true),
                    ingestion = table.Column<List<int>>(type: "integer[]", nullable: true),
                    sensitizer = table.Column<List<int>>(type: "integer[]", nullable: true),
                    carcinogen = table.Column<List<int>>(type: "integer[]", nullable: true),
                    reproductive = table.Column<List<int>>(type: "integer[]", nullable: true),
                    other = table.Column<List<int>>(type: "integer[]", nullable: true),
                    ppe_glove = table.Column<List<string>>(type: "text[]", nullable: true),
                    ppe_suit = table.Column<List<string>>(type: "text[]", nullable: true),
                    ppe_eyewear = table.Column<List<string>>(type: "text[]", nullable: true),
                    ppe_respiratory = table.Column<List<string>>(type: "text[]", nullable: true),
                    oels = table.Column<string>(type: "text", nullable: true),
                    emergency_response = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    risk_score = table.Column<int>(type: "integer", nullable: true),
                    person_performing_assessment_username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    person_performing_assessment_displayname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    date_conducted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    person_performing_review_username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    person_performing_review_displayname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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
                name: "controls_recommended",
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
                    table.PrimaryKey("PK_controls_recommended", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exposure_rating",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_exposure_rating", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exposure_type",
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
                    table.PrimaryKey("PK_exposure_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "frequency_of_task",
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
                    table.PrimaryKey("PK_frequency_of_task", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "has_agent_been_changed",
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
                    table.PrimaryKey("PK_has_agent_been_changed", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hazard_codes",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_hazard_codes", x => x.id);
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
                name: "health_effect_rating",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_health_effect_rating", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ih_chemical",
                schema: "ehs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CasNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PubChemCid = table.Column<int>(type: "integer", nullable: true),
                    PreferredName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ih_chemical", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "location",
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
                    table.PrimaryKey("PK_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_data_required",
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
                    table.PrimaryKey("PK_monitoring_data_required", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "number_of_workers",
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
                    table.PrimaryKey("PK_number_of_workers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "occupational_exposure_limit",
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
                    table.PrimaryKey("PK_occupational_exposure_limit", x => x.id);
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
                name: "route_of_entry",
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
                    table.PrimaryKey("PK_route_of_entry", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "seg_risk_assessment",
                schema: "ehs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    location = table.Column<string>(type: "text", nullable: false),
                    exposure_type = table.Column<string>(type: "text", nullable: false),
                    agent = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    task = table.Column<string>(type: "text", nullable: false),
                    frequency_of_task = table.Column<string>(type: "text", nullable: false),
                    duration_of_task = table.Column<TimeSpan>(type: "interval", nullable: false),
                    oel = table.Column<string>(type: "text", nullable: true),
                    route_of_entry = table.Column<List<string>>(type: "text[]", nullable: false),
                    controls_recommended = table.Column<List<string>>(type: "text[]", nullable: true),
                    exposure_levels_acceptable = table.Column<string>(type: "text", nullable: true),
                    assessment_methods_used = table.Column<string>(type: "text", nullable: false),
                    seg_number_of_workers = table.Column<string>(type: "text", nullable: false),
                    has_agent_been_changed = table.Column<string>(type: "text", nullable: true),
                    person_performing_assessment_username = table.Column<string>(type: "text", nullable: false),
                    person_performing_assessment_displayname = table.Column<string>(type: "text", nullable: true),
                    date_conducted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_reviewed = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    exposure_rating = table.Column<int>(type: "integer", nullable: true),
                    exposure_rating_description = table.Column<string>(type: "text", nullable: true),
                    health_effect_rating = table.Column<int>(type: "integer", nullable: true),
                    health_effect_rating_description = table.Column<string>(type: "text", nullable: true),
                    risk_score = table.Column<int>(type: "integer", nullable: true, computedColumnSql: "\"exposure_rating\" * \"health_effect_rating\"", stored: true),
                    additional_notes = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_seg_risk_assessment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "seg_role",
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
                    table.PrimaryKey("PK_seg_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task",
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
                    table.PrimaryKey("PK_task", x => x.id);
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

            migrationBuilder.CreateTable(
                name: "yes_no",
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
                    table.PrimaryKey("PK_yes_no", x => x.id);
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

            migrationBuilder.CreateTable(
                name: "ih_chemical_hazard",
                schema: "ehs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IhChemicalId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ih_chemical_hazard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ih_chemical_hazard_ih_chemical_IhChemicalId",
                        column: x => x.IhChemicalId,
                        principalSchema: "ehs",
                        principalTable: "ih_chemical",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ih_chemical_oel",
                schema: "ehs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IhChemicalId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ih_chemical_oel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ih_chemical_oel_ih_chemical_IhChemicalId",
                        column: x => x.IhChemicalId,
                        principalSchema: "ehs",
                        principalTable: "ih_chemical",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ih_chemical_property",
                schema: "ehs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IhChemicalId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ih_chemical_property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ih_chemical_property_ih_chemical_IhChemicalId",
                        column: x => x.IhChemicalId,
                        principalSchema: "ehs",
                        principalTable: "ih_chemical",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ih_chemical_sampling_method",
                schema: "ehs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IhChemicalId = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    MethodId = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ih_chemical_sampling_method", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ih_chemical_sampling_method_ih_chemical_IhChemicalId",
                        column: x => x.IhChemicalId,
                        principalSchema: "ehs",
                        principalTable: "ih_chemical",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ih_chemical_synonym",
                schema: "ehs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IhChemicalId = table.Column<int>(type: "integer", nullable: false),
                    Synonym = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ih_chemical_synonym", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ih_chemical_synonym_ih_chemical_IhChemicalId",
                        column: x => x.IhChemicalId,
                        principalSchema: "ehs",
                        principalTable: "ih_chemical",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_composition_chemicalriskassessmentid",
                schema: "ehs",
                table: "chemical_composition",
                column: "chemical_risk_assessment_id");

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_CasNumber",
                schema: "ehs",
                table: "ih_chemical",
                column: "CasNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_hazard_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_hazard",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_hazard_IhChemicalId_Source_Code",
                schema: "ehs",
                table: "ih_chemical_hazard",
                columns: new[] { "IhChemicalId", "Source", "Code" });

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_oel_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_oel",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_oel_IhChemicalId_Source_Type",
                schema: "ehs",
                table: "ih_chemical_oel",
                columns: new[] { "IhChemicalId", "Source", "Type" });

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_property_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_property",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_property_IhChemicalId_Key",
                schema: "ehs",
                table: "ih_chemical_property",
                columns: new[] { "IhChemicalId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_sampling_method_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_sampling_method",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_sampling_method_IhChemicalId_Source_MethodId",
                schema: "ehs",
                table: "ih_chemical_sampling_method",
                columns: new[] { "IhChemicalId", "Source", "MethodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_synonym_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_synonym",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_synonym_IhChemicalId_Synonym",
                schema: "ehs",
                table: "ih_chemical_synonym",
                columns: new[] { "IhChemicalId", "Synonym" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acute_chronic",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "agent",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "area",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "assessment_methods_used",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "chemical_composition",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "controls_recommended",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "exposure_rating",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "exposure_type",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "frequency_of_task",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "has_agent_been_changed",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "hazard_codes",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "hazardous",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "health_effect_rating",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ih_chemical_hazard",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ih_chemical_oel",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ih_chemical_property",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ih_chemical_sampling_method",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ih_chemical_synonym",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "location",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "monitoring_data_required",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "number_of_workers",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "occupational_exposure_limit",
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
                name: "route_of_entry",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "seg_risk_assessment",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "seg_role",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "task",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "use",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "yes_no",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "chemical_risk_assessment",
                schema: "ehs");

            migrationBuilder.DropTable(
                name: "ih_chemical",
                schema: "ehs");
        }
    }
}

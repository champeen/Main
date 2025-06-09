using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seg_risk_assessments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    location = table.Column<string>(type: "text", nullable: false),
                    exposure_type = table.Column<string>(type: "text", nullable: false),
                    agent = table.Column<string>(type: "text", nullable: false),
                    seg_role = table.Column<string>(type: "text", nullable: false),
                    task = table.Column<string>(type: "text", nullable: false),
                    oel = table.Column<string>(type: "text", nullable: false),
                    acute_chronic = table.Column<string>(type: "text", nullable: false),
                    route_of_entry = table.Column<string>(type: "text", nullable: false),
                    frequency_of_task = table.Column<string>(type: "text", nullable: false),
                    duration_of_task = table.Column<string>(type: "text", nullable: false),
                    monitoring_data_required = table.Column<string>(type: "text", nullable: false),
                    controls_recommended = table.Column<string>(type: "text", nullable: false),
                    exposure_levels_acceptable = table.Column<string>(type: "text", nullable: false),
                    date_conducted = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    assessment_methods_used = table.Column<string>(type: "text", nullable: false),
                    seg_number_of_workers = table.Column<int>(type: "integer", nullable: false),
                    has_agent_been_changed = table.Column<string>(type: "text", nullable: false),
                    person_performing_assessment = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_seg_risk_assessments", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seg_risk_assessments");
        }
    }
}

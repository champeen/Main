using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class IhChem_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ih_chemical",
                schema: "ehs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CasNumber = table.Column<string>(type: "text", nullable: false),
                    PubChemCid = table.Column<int>(type: "integer", nullable: true),
                    PreferredName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ih_chemical", x => x.Id);
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
                name: "IX_ih_chemical_CasNumber",
                schema: "ehs",
                table: "ih_chemical",
                column: "CasNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_hazard_IhChemicalId_Source_Code",
                schema: "ehs",
                table: "ih_chemical_hazard",
                columns: new[] { "IhChemicalId", "Source", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_oel_IhChemicalId_Source_Type",
                schema: "ehs",
                table: "ih_chemical_oel",
                columns: new[] { "IhChemicalId", "Source", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_property_IhChemicalId_Key",
                schema: "ehs",
                table: "ih_chemical_property",
                columns: new[] { "IhChemicalId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ih_chemical_sampling_method_IhChemicalId_Source_MethodId",
                schema: "ehs",
                table: "ih_chemical_sampling_method",
                columns: new[] { "IhChemicalId", "Source", "MethodId" },
                unique: true);

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
                name: "ih_chemical",
                schema: "ehs");
        }
    }
}

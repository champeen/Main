using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHS.Migrations
{
    /// <inheritdoc />
    public partial class AddChemicalIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_synonym_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_synonym",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_sampling_method_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_sampling_method",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_property_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_property",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_oel_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_oel",
                column: "IhChemicalId");

            migrationBuilder.CreateIndex(
                name: "ix_ih_chemical_hazard_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_hazard",
                column: "IhChemicalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ih_chemical_synonym_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_synonym");

            migrationBuilder.DropIndex(
                name: "ix_ih_chemical_sampling_method_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_sampling_method");

            migrationBuilder.DropIndex(
                name: "ix_ih_chemical_property_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_property");

            migrationBuilder.DropIndex(
                name: "ix_ih_chemical_oel_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_oel");

            migrationBuilder.DropIndex(
                name: "ix_ih_chemical_hazard_ihchemicalid",
                schema: "ehs",
                table: "ih_chemical_hazard");
        }
    }
}

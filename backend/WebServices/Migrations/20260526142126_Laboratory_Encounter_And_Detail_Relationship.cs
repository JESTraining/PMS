using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServices.Migrations
{
    /// <inheritdoc />
    public partial class Laboratory_Encounter_And_Detail_Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncounterLaboratoriesDetail_EncounterLaboratories_EncounterLaboratoriesId",
                table: "EncounterLaboratoriesDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_EncounterLaboratoriesDetail_Laboratories_LaboratoryId",
                table: "EncounterLaboratoriesDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EncounterLaboratoriesDetail",
                table: "EncounterLaboratoriesDetail");

            migrationBuilder.RenameTable(
                name: "EncounterLaboratoriesDetail",
                newName: "EncounterLaboratoriesDetails");

            migrationBuilder.RenameIndex(
                name: "IX_EncounterLaboratoriesDetail_LaboratoryId",
                table: "EncounterLaboratoriesDetails",
                newName: "IX_EncounterLaboratoriesDetails_LaboratoryId");

            migrationBuilder.RenameIndex(
                name: "IX_EncounterLaboratoriesDetail_EncounterLaboratoriesId",
                table: "EncounterLaboratoriesDetails",
                newName: "IX_EncounterLaboratoriesDetails_EncounterLaboratoriesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EncounterLaboratoriesDetails",
                table: "EncounterLaboratoriesDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EncounterLaboratoriesDetails_EncounterLaboratories_EncounterLaboratoriesId",
                table: "EncounterLaboratoriesDetails",
                column: "EncounterLaboratoriesId",
                principalTable: "EncounterLaboratories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EncounterLaboratoriesDetails_Laboratories_LaboratoryId",
                table: "EncounterLaboratoriesDetails",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncounterLaboratoriesDetails_EncounterLaboratories_EncounterLaboratoriesId",
                table: "EncounterLaboratoriesDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_EncounterLaboratoriesDetails_Laboratories_LaboratoryId",
                table: "EncounterLaboratoriesDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EncounterLaboratoriesDetails",
                table: "EncounterLaboratoriesDetails");

            migrationBuilder.RenameTable(
                name: "EncounterLaboratoriesDetails",
                newName: "EncounterLaboratoriesDetail");

            migrationBuilder.RenameIndex(
                name: "IX_EncounterLaboratoriesDetails_LaboratoryId",
                table: "EncounterLaboratoriesDetail",
                newName: "IX_EncounterLaboratoriesDetail_LaboratoryId");

            migrationBuilder.RenameIndex(
                name: "IX_EncounterLaboratoriesDetails_EncounterLaboratoriesId",
                table: "EncounterLaboratoriesDetail",
                newName: "IX_EncounterLaboratoriesDetail_EncounterLaboratoriesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EncounterLaboratoriesDetail",
                table: "EncounterLaboratoriesDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EncounterLaboratoriesDetail_EncounterLaboratories_EncounterLaboratoriesId",
                table: "EncounterLaboratoriesDetail",
                column: "EncounterLaboratoriesId",
                principalTable: "EncounterLaboratories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EncounterLaboratoriesDetail_Laboratories_LaboratoryId",
                table: "EncounterLaboratoriesDetail",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id");
        }
    }
}

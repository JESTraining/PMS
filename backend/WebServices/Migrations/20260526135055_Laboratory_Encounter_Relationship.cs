using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServices.Migrations
{
    /// <inheritdoc />
    public partial class Laboratory_Encounter_Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Laboratories_Encounters_Id",
                table: "Laboratories");

            migrationBuilder.CreateTable(
                name: "EncounterLaboratories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOrdered = table.Column<DateOnly>(type: "date", nullable: false),
                    LaboratoryStatus = table.Column<int>(type: "int", nullable: false),
                    EncounterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterLaboratories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncounterLaboratories_Encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "Encounters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EncounterLaboratoriesDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncounterLaboratoriesId = table.Column<int>(type: "int", nullable: true),
                    LaboratoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterLaboratoriesDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncounterLaboratoriesDetail_EncounterLaboratories_EncounterLaboratoriesId",
                        column: x => x.EncounterLaboratoriesId,
                        principalTable: "EncounterLaboratories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EncounterLaboratoriesDetail_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "Laboratories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncounterLaboratories_EncounterId",
                table: "EncounterLaboratories",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterLaboratoriesDetail_EncounterLaboratoriesId",
                table: "EncounterLaboratoriesDetail",
                column: "EncounterLaboratoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterLaboratoriesDetail_LaboratoryId",
                table: "EncounterLaboratoriesDetail",
                column: "LaboratoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncounterLaboratoriesDetail");

            migrationBuilder.DropTable(
                name: "EncounterLaboratories");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Laboratories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Laboratories_Encounters_Id",
                table: "Laboratories",
                column: "Id",
                principalTable: "Encounters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

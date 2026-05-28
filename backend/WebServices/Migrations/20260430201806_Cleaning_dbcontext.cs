using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServices.Migrations
{
    /// <inheritdoc />
    public partial class Cleaning_dbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.DropColumn(
                name: "Dosage",
                table: "DBPrescriptions");

             migrationBuilder.DropColumn(
                name: "Medication",
                table: "DBPrescriptions");

             migrationBuilder.DropColumn(
                name: "Refils",
                table: "DBPrescriptions");

            migrationBuilder.CreateTable(
                name: "PrescriptionMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Refills = table.Column<int>(type: "int", nullable: false),
                    PrescriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionMedications_DBPrescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "DBPrescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionMedications_PrescriptionId",
                table: "PrescriptionMedications",
                column: "PrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionMedications");

            migrationBuilder.AddColumn<string>(
                name: "Dosage",
                table: "DBPrescriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Medication",
                table: "DBPrescriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Refils",
                table: "DBPrescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

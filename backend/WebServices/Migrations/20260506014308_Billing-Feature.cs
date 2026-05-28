using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServices.Migrations
{
    /// <inheritdoc />
    public partial class BillingFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EncounterId",
                table: "DBInvoices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvoiceDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetail_DBInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "DBInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DBInvoices_EncounterId",
                table: "DBInvoices",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail_InvoiceId",
                table: "InvoiceDetail",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_DBInvoices_Encounters_EncounterId",
                table: "DBInvoices",
                column: "EncounterId",
                principalTable: "Encounters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DBInvoices_Encounters_EncounterId",
                table: "DBInvoices");

            migrationBuilder.DropTable(
                name: "InvoiceDetail");

            migrationBuilder.DropIndex(
                name: "IX_DBInvoices_EncounterId",
                table: "DBInvoices");

            migrationBuilder.DropColumn(
                name: "EncounterId",
                table: "DBInvoices");
        }
    }
}

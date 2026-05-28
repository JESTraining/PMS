using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServices.Migrations
{
    /// <inheritdoc />
    public partial class Laboratory_Strcuture_And_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Laboratories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: true),
                    TimeToCompleteInHours = table.Column<int>(type: "int", nullable: false),
                    NoFoodBeforeExecuted = table.Column<bool>(type: "bit", nullable: false),
                    LiquidIngestionBeforeExecuted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Laboratories_Encounters_Id",
                        column: x => x.Id,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Laboratories");
        }
    }
}

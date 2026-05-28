using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServices.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataFixing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DBPatients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "DBPatients",
                columns: new[] { "Id", "CreatedAt", "DateOfBirth", "Email", "FirstName", "LastName", "Phone" },
                values: new object[] { -1, new DateTime(2026, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "juan@test.com", "Juan", "Perez", "12345678" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DBPatients",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.InsertData(
                table: "DBPatients",
                columns: new[] { "Id", "CreatedAt", "DateOfBirth", "Email", "FirstName", "LastName", "Phone" },
                values: new object[] { 1, new DateTime(2026, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "juan@test.com", "Juan", "Perez", "12345678" });
        }
    }
}

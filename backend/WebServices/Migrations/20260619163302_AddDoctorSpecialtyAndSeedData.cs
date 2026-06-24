using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebServices.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorSpecialtyAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "DBDoctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Specialty",
                value: "Pediatrics");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreationDate", "Email", "IsActive", "Name", "Password", "Role", "UserName" },
                values: new object[,]
                {
                    { -12, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "pablo.rodriguez@pms.com", true, "Pablo Rodriguez", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor10" },
                    { -11, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "carmen.jimenez@pms.com", true, "Carmen Jimenez", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor9" },
                    { -10, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "roberto.morales@pms.com", true, "Roberto Morales", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor8" },
                    { -9, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "elena.vasquez@pms.com", true, "Elena Vasquez", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor7" },
                    { -8, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "maria.santos@pms.com", true, "Maria Santos", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor6" },
                    { -7, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "luis.garcia@pms.com", true, "Luis Garcia", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor5" },
                    { -6, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "ana.torres@pms.com", true, "Ana Torres", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor4" },
                    { -5, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "carlos.mendez@pms.com", true, "Carlos Mendez", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor3" },
                    { -4, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "sofia.ramirez@pms.com", true, "Sofia Ramirez", "387D800C0CC82412028CE6435ABC708A52C075D8ED8F9854FBE24691B5E46D8C", "DOCTOR", "doctor2" }
                });

            migrationBuilder.InsertData(
                table: "DBDoctors",
                columns: new[] { "Id", "Name", "Specialty", "UserId" },
                values: new object[,]
                {
                    { 2, "Sofia Ramirez", "General Medicine", -4 },
                    { 3, "Carlos Mendez", "General Medicine", -5 },
                    { 4, "Ana Torres", "Cardiology", -6 },
                    { 5, "Luis Garcia", "General Medicine", -7 },
                    { 6, "Maria Santos", "Pediatrics", -8 },
                    { 7, "Elena Vasquez", "Neurology", -9 },
                    { 8, "Roberto Morales", "Cardiology", -10 },
                    { 9, "Carmen Jimenez", "Dermatology", -11 },
                    { 10, "Pablo Rodriguez", "Neurology", -12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "DBDoctors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "DBDoctors");
        }
    }
}

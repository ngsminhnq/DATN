using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace HRemployee.Migrations
{

    public partial class AddContracts : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Contracts",
                columns: new[] { "Id", "ContractCode", "ContractType", "CreatedAt", "EmployeeId", "EndDate", "Note", "SalaryPercent", "StartDate", "Status", "TerminatedAt" },
                values: new object[,]
                {
                    { 1, "HD001", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 100, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null },
                    { 2, "HD002", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 100, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null },
                    { 3, "HD003", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 100, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null },
                    { 4, "HD004", 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 100, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
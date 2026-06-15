using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace HRemployee.Migrations
{

    public partial class UpdateSeedData4Accounts : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "Avatar", "CreatedAt", "DateOfBirth", "DepartmentId", "Email", "EmployeeCode", "FullName", "Gender", "HireDate", "IsDeleted", "ManagerId", "Phone", "PositionId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, "blockmanager@company.com", "NV002", "Giám Đốc Khối", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, 2, 1, null },
                    { 3, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, "centermanager@company.com", "NV003", "Trưởng Ban", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, 2, 1, null },
                    { 4, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, "employee@company.com", "NV004", "Nhân Viên", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null, 3, 1, null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "EmployeeId", "IsActive", "Password", "RoleId", "Username" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "blockmanager@company.com", 2, true, "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K", 2, "blockmanager" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "centermanager@company.com", 3, true, "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K", 3, "centermanager" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "employee@company.com", 4, true, "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K", 4, "employee" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$umDEKg3yORpv174r7kzKxO7Z.BVbw0HDzb44jCsvgjHGGn5rM6/Ky");
        }
    }
}
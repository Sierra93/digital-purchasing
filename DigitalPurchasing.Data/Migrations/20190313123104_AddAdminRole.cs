using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddAdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"),
                column: "ConcurrencyStamp",
                value: "523a1a9dfa7045ed9ca01ddf3f037779");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("8de94260-ade7-4062-aec1-a1568f62ecd1"), "8de94260ade74062aec1a1568f62ecd1", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8de94260-ade7-4062-aec1-a1568f62ecd1"));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"),
                column: "ConcurrencyStamp",
                value: "954d8b5a-8d08-4ca6-929a-7253b508919f");
        }
    }
}

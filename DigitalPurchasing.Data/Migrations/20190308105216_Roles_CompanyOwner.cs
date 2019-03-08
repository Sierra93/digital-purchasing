using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Roles_CompanyOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"), "3f42ebb3-59f4-4b86-8aab-ba0275407370", "CompanyOwner", "COMPANYOWNER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"));
        }
    }
}

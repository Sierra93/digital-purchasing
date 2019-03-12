using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class AddRolesToOwners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO [UserRoles] SELECT Id AS 'UserId', '523a1a9d-fa70-45ed-9ca0-1ddf3f037779' as 'RoleId' FROM [Users] AS [U] " +
                "WHERE NOT EXISTS (SELECT * FROM [UserRoles] AS [UR] WHERE UR.UserId = U.Id)");
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"),
                column: "ConcurrencyStamp",
                value: "954d8b5a-8d08-4ca6-929a-7253b508919f");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"),
                column: "ConcurrencyStamp",
                value: "36bb2dd7-18be-4dfc-b49d-8660a553aa6b");
        }
    }
}

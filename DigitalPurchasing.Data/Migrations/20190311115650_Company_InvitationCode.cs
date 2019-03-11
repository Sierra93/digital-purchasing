using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Company_InvitationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitationCode",
                table: "Companies",
                nullable: true);

            migrationBuilder.Sql("UPDATE Companies SET InvitationCode = LOWER(REPLACE(NEWID(), '-', ''))");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"),
                column: "ConcurrencyStamp",
                value: "36bb2dd7-18be-4dfc-b49d-8660a553aa6b");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitationCode",
                table: "Companies");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("523a1a9d-fa70-45ed-9ca0-1ddf3f037779"),
                column: "ConcurrencyStamp",
                value: "3f42ebb3-59f4-4b86-8aab-ba0275407370");
        }
    }
}

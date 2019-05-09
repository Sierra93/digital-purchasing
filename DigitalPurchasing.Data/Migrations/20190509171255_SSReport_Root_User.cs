using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSReport_Root_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RootId",
                table: "SSReports",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "SSReports",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SSReports_RootId",
                table: "SSReports",
                column: "RootId");

            migrationBuilder.CreateIndex(
                name: "IX_SSReports_UserId",
                table: "SSReports",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SSReports_Roots_RootId",
                table: "SSReports",
                column: "RootId",
                principalTable: "Roots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SSReports_Users_UserId",
                table: "SSReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSReports_Roots_RootId",
                table: "SSReports");

            migrationBuilder.DropForeignKey(
                name: "FK_SSReports_Users_UserId",
                table: "SSReports");

            migrationBuilder.DropIndex(
                name: "IX_SSReports_RootId",
                table: "SSReports");

            migrationBuilder.DropIndex(
                name: "IX_SSReports_UserId",
                table: "SSReports");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "SSReports");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SSReports");
        }
    }
}

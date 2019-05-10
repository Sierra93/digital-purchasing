using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSCustomer_Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [SSReports]");
            migrationBuilder.Sql("DELETE FROM [SSCustomers]");

            migrationBuilder.AddColumn<Guid>(
                name: "ReportId",
                table: "SSCustomers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SSCustomers_ReportId",
                table: "SSCustomers",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_SSCustomers_SSReports_ReportId",
                table: "SSCustomers",
                column: "ReportId",
                principalTable: "SSReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSCustomers_SSReports_ReportId",
                table: "SSCustomers");

            migrationBuilder.DropIndex(
                name: "IX_SSCustomers_ReportId",
                table: "SSCustomers");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "SSCustomers");
        }
    }
}

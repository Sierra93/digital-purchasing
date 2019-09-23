using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSSupplier_Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReportId",
                table: "SSSuppliers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SSSuppliers_ReportId",
                table: "SSSuppliers",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_SSSuppliers_SSReports_ReportId",
                table: "SSSuppliers",
                column: "ReportId",
                principalTable: "SSReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSSuppliers_SSReports_ReportId",
                table: "SSSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_SSSuppliers_ReportId",
                table: "SSSuppliers");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "SSSuppliers");
        }
    }
}

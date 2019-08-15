using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class UomConversionRate_NomenclatureAlternative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NomenclatureAlternativeId",
                table: "UomConversionRates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UomConversionRates_NomenclatureAlternativeId",
                table: "UomConversionRates",
                column: "NomenclatureAlternativeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UomConversionRates_NomenclatureAlternatives_NomenclatureAlternativeId",
                table: "UomConversionRates",
                column: "NomenclatureAlternativeId",
                principalTable: "NomenclatureAlternatives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UomConversionRates_NomenclatureAlternatives_NomenclatureAlternativeId",
                table: "UomConversionRates");

            migrationBuilder.DropIndex(
                name: "IX_UomConversionRates_NomenclatureAlternativeId",
                table: "UomConversionRates");

            migrationBuilder.DropColumn(
                name: "NomenclatureAlternativeId",
                table: "UomConversionRates");
        }
    }
}

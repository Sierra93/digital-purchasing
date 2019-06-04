using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Link_NomenclatureComparisonDatas_WithAlternatives : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NomenclatureComparisonDatas_NomenclatureId",
                table: "NomenclatureComparisonDatas");

            migrationBuilder.AddColumn<Guid>(
                name: "NomenclatureAlternativeId",
                table: "NomenclatureComparisonDatas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureComparisonDatas_NomenclatureAlternativeId",
                table: "NomenclatureComparisonDatas",
                column: "NomenclatureAlternativeId",
                unique: true,
                filter: "[NomenclatureAlternativeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureComparisonDatas_NomenclatureId",
                table: "NomenclatureComparisonDatas",
                column: "NomenclatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureComparisonDatas_NomenclatureAlternatives_NomenclatureAlternativeId",
                table: "NomenclatureComparisonDatas",
                column: "NomenclatureAlternativeId",
                principalTable: "NomenclatureAlternatives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureComparisonDatas_NomenclatureAlternatives_NomenclatureAlternativeId",
                table: "NomenclatureComparisonDatas");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureComparisonDatas_NomenclatureAlternativeId",
                table: "NomenclatureComparisonDatas");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureComparisonDatas_NomenclatureId",
                table: "NomenclatureComparisonDatas");

            migrationBuilder.DropColumn(
                name: "NomenclatureAlternativeId",
                table: "NomenclatureComparisonDatas");

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureComparisonDatas_NomenclatureId",
                table: "NomenclatureComparisonDatas",
                column: "NomenclatureId",
                unique: true);
        }
    }
}

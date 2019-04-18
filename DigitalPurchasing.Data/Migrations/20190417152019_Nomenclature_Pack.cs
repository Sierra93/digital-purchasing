using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Nomenclature_Pack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PackUomId",
                table: "Nomenclatures",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PackUomValue",
                table: "Nomenclatures",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_PackUomId",
                table: "Nomenclatures",
                column: "PackUomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_PackUomId",
                table: "Nomenclatures",
                column: "PackUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nomenclatures_UnitsOfMeasurements_PackUomId",
                table: "Nomenclatures");

            migrationBuilder.DropIndex(
                name: "IX_Nomenclatures_PackUomId",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "PackUomId",
                table: "Nomenclatures");

            migrationBuilder.DropColumn(
                name: "PackUomValue",
                table: "Nomenclatures");
        }
    }
}

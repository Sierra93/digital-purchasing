using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class NewColumns_NomenclatureAlternative_PackUoms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PackUomId",
                table: "NomenclatureAlternatives",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PackUomValue",
                table: "NomenclatureAlternatives",
                type: "decimal(18, 4)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NomenclatureAlternatives_PackUomId",
                table: "NomenclatureAlternatives",
                column: "PackUomId");

            migrationBuilder.AddForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_PackUomId",
                table: "NomenclatureAlternatives",
                column: "PackUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NomenclatureAlternatives_UnitsOfMeasurements_PackUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropIndex(
                name: "IX_NomenclatureAlternatives_PackUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "PackUomId",
                table: "NomenclatureAlternatives");

            migrationBuilder.DropColumn(
                name: "PackUomValue",
                table: "NomenclatureAlternatives");
        }
    }
}

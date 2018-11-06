using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOfferItem_PropsForMatchingUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommonFactor",
                table: "SupplierOfferItems",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NomenclatureFactor",
                table: "SupplierOfferItems",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "NomenclatureId",
                table: "SupplierOfferItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RawUomMatchId",
                table: "SupplierOfferItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferItems_NomenclatureId",
                table: "SupplierOfferItems",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOfferItems_RawUomMatchId",
                table: "SupplierOfferItems",
                column: "RawUomMatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOfferItems_Nomenclatures_NomenclatureId",
                table: "SupplierOfferItems",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOfferItems_UnitsOfMeasurements_RawUomMatchId",
                table: "SupplierOfferItems",
                column: "RawUomMatchId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOfferItems_Nomenclatures_NomenclatureId",
                table: "SupplierOfferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOfferItems_UnitsOfMeasurements_RawUomMatchId",
                table: "SupplierOfferItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOfferItems_NomenclatureId",
                table: "SupplierOfferItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOfferItems_RawUomMatchId",
                table: "SupplierOfferItems");

            migrationBuilder.DropColumn(
                name: "CommonFactor",
                table: "SupplierOfferItems");

            migrationBuilder.DropColumn(
                name: "NomenclatureFactor",
                table: "SupplierOfferItems");

            migrationBuilder.DropColumn(
                name: "NomenclatureId",
                table: "SupplierOfferItems");

            migrationBuilder.DropColumn(
                name: "RawUomMatchId",
                table: "SupplierOfferItems");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOfferItem_RawUomToRawUomStr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOfferItems_UnitsOfMeasurements_RawUomMatchId",
                table: "SupplierOfferItems");

            migrationBuilder.RenameColumn(
                name: "RawUomMatchId",
                table: "SupplierOfferItems",
                newName: "RawUomId");

            migrationBuilder.RenameColumn(
                name: "RawUom",
                table: "SupplierOfferItems",
                newName: "RawUomStr");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierOfferItems_RawUomMatchId",
                table: "SupplierOfferItems",
                newName: "IX_SupplierOfferItems_RawUomId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOfferItems_UnitsOfMeasurements_RawUomId",
                table: "SupplierOfferItems",
                column: "RawUomId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOfferItems_UnitsOfMeasurements_RawUomId",
                table: "SupplierOfferItems");

            migrationBuilder.RenameColumn(
                name: "RawUomStr",
                table: "SupplierOfferItems",
                newName: "RawUom");

            migrationBuilder.RenameColumn(
                name: "RawUomId",
                table: "SupplierOfferItems",
                newName: "RawUomMatchId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierOfferItems_RawUomId",
                table: "SupplierOfferItems",
                newName: "IX_SupplierOfferItems_RawUomMatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOfferItems_UnitsOfMeasurements_RawUomMatchId",
                table: "SupplierOfferItems",
                column: "RawUomMatchId",
                principalTable: "UnitsOfMeasurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

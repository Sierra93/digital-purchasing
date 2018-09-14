using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class PurchasingRequestItem_RawItemAndNomenclature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "RawPurchasingRequestItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "NomenclatureId",
                table: "PurchasingRequestItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RawItemId",
                table: "PurchasingRequestItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 32, 32, 823, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 32, 32, 823, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 32, 32, 823, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UomConversionRates",
                keyColumn: "Id",
                keyValue: new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 32, 32, 825, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequestItems_NomenclatureId",
                table: "PurchasingRequestItems",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems",
                column: "RawItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingRequestItems_Nomenclatures_NomenclatureId",
                table: "PurchasingRequestItems",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingRequestItems_RawPurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems",
                column: "RawItemId",
                principalTable: "RawPurchasingRequestItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingRequestItems_Nomenclatures_NomenclatureId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingRequestItems_RawPurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchasingRequestItems_NomenclatureId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "RawPurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "NomenclatureId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "RawItemId",
                table: "PurchasingRequestItems");

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UomConversionRates",
                keyColumn: "Id",
                keyValue: new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc));
        }
    }
}

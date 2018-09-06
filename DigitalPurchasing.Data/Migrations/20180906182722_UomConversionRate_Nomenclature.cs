using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class UomConversionRate_Nomenclature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NomenclatureId",
                table: "UomConversionRates",
                nullable: true);

            migrationBuilder.InsertData(
                table: "UnitsOfMeasurements",
                columns: new[] { "Id", "CreatedOn", "Name", "OwnerId" },
                values: new object[] { new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"), new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc), "шт", null });

            migrationBuilder.InsertData(
                table: "UnitsOfMeasurements",
                columns: new[] { "Id", "CreatedOn", "Name", "OwnerId" },
                values: new object[] { new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"), new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc), "тыс шт", null });

            migrationBuilder.InsertData(
                table: "UnitsOfMeasurements",
                columns: new[] { "Id", "CreatedOn", "Name", "OwnerId" },
                values: new object[] { new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"), new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc), "кг", null });

            migrationBuilder.InsertData(
                table: "UomConversionRates",
                columns: new[] { "Id", "CreatedOn", "Factor", "FromUomId", "NomenclatureId", "OwnerId", "ToUomId" },
                values: new object[] { new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"), new DateTime(2018, 9, 6, 18, 27, 22, 589, DateTimeKind.Utc), 1000m, new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"), null, null, new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b") });

            migrationBuilder.CreateIndex(
                name: "IX_UomConversionRates_NomenclatureId",
                table: "UomConversionRates",
                column: "NomenclatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_UomConversionRates_Nomenclatures_NomenclatureId",
                table: "UomConversionRates",
                column: "NomenclatureId",
                principalTable: "Nomenclatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UomConversionRates_Nomenclatures_NomenclatureId",
                table: "UomConversionRates");

            migrationBuilder.DropIndex(
                name: "IX_UomConversionRates_NomenclatureId",
                table: "UomConversionRates");

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"));

            migrationBuilder.DeleteData(
                table: "UomConversionRates",
                keyColumn: "Id",
                keyValue: new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"));

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"));

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"));

            migrationBuilder.DropColumn(
                name: "NomenclatureId",
                table: "UomConversionRates");
        }
    }
}

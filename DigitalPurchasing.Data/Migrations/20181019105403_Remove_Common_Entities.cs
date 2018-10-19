using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Remove_Common_Entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [PurchaseRequestItems]", true);
            migrationBuilder.Sql("DELETE FROM [PurchaseRequests];DELETE FROM [QuotationRequests];DELETE FROM [Deliveries]", true);
            migrationBuilder.Sql("DELETE FROM [NomenclatureAlternatives];DELETE FROM [UomConversionRates];DELETE FROM [Nomenclatures];", true);
            migrationBuilder.Sql("DELETE FROM [UnitsOfMeasurements]", true);

            migrationBuilder.DropForeignKey(
                name: "FK_UnitsOfMeasurements_Companies_OwnerId",
                table: "UnitsOfMeasurements");

            migrationBuilder.DropForeignKey(
                name: "FK_UomConversionRates_Companies_OwnerId",
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

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "UomConversionRates",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "UnitsOfMeasurements",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitsOfMeasurements_Companies_OwnerId",
                table: "UnitsOfMeasurements",
                column: "OwnerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UomConversionRates_Companies_OwnerId",
                table: "UomConversionRates",
                column: "OwnerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitsOfMeasurements_Companies_OwnerId",
                table: "UnitsOfMeasurements");

            migrationBuilder.DropForeignKey(
                name: "FK_UomConversionRates_Companies_OwnerId",
                table: "UomConversionRates");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "UomConversionRates",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "UnitsOfMeasurements",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.InsertData(
                table: "UnitsOfMeasurements",
                columns: new[] { "Id", "CreatedOn", "IsDeleted", "Name", "OwnerId" },
                values: new object[] { new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"), new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), false, "шт", null });

            migrationBuilder.InsertData(
                table: "UnitsOfMeasurements",
                columns: new[] { "Id", "CreatedOn", "IsDeleted", "Name", "OwnerId" },
                values: new object[] { new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"), new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), false, "тыс шт", null });

            migrationBuilder.InsertData(
                table: "UnitsOfMeasurements",
                columns: new[] { "Id", "CreatedOn", "IsDeleted", "Name", "OwnerId" },
                values: new object[] { new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"), new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), false, "кг", null });

            migrationBuilder.InsertData(
                table: "UomConversionRates",
                columns: new[] { "Id", "CreatedOn", "Factor", "FromUomId", "NomenclatureId", "OwnerId", "ToUomId" },
                values: new object[] { new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"), new DateTime(2018, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), 1000m, new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"), null, null, new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b") });

            migrationBuilder.AddForeignKey(
                name: "FK_UnitsOfMeasurements_Companies_OwnerId",
                table: "UnitsOfMeasurements",
                column: "OwnerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UomConversionRates_Companies_OwnerId",
                table: "UomConversionRates",
                column: "OwnerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

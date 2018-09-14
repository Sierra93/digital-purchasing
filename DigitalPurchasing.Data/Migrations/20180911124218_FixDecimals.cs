using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class FixDecimals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Factor",
                table: "UomConversionRates",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "RawPurchasingRequestItems",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "ResourceUomValue",
                table: "Nomenclatures",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "MassUomValue",
                table: "Nomenclatures",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 42, 18, 234, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 42, 18, 234, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 42, 18, 234, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UomConversionRates",
                keyColumn: "Id",
                keyValue: new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 12, 42, 18, 235, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Factor",
                table: "UomConversionRates",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "RawPurchasingRequestItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ResourceUomValue",
                table: "Nomenclatures",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MassUomValue",
                table: "Nomenclatures",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

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
        }
    }
}

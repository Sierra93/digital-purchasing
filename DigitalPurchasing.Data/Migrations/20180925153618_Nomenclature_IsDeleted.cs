using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Nomenclature_IsDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Nomenclatures",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 25, 15, 36, 18, 431, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 25, 15, 36, 18, 431, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 25, 15, 36, 18, 431, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UomConversionRates",
                keyColumn: "Id",
                keyValue: new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 25, 15, 36, 18, 432, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Nomenclatures");

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 18, 8, 55, 51, 527, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 18, 8, 55, 51, 529, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 18, 8, 55, 51, 529, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UomConversionRates",
                keyColumn: "Id",
                keyValue: new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 18, 8, 55, 51, 529, DateTimeKind.Utc));
        }
    }
}

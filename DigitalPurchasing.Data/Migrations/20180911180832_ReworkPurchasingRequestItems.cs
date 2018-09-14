using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReworkPurchasingRequestItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingRequestItems_RawPurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropTable(
                name: "RawPurchasingRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "RawItemId",
                table: "PurchasingRequestItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "NomenclatureId",
                table: "PurchasingRequestItems",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "PurchasingRequestItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RawCode",
                table: "PurchasingRequestItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawName",
                table: "PurchasingRequestItems",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RawQty",
                table: "PurchasingRequestItems",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RawUom",
                table: "PurchasingRequestItems",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("0a45a476-e69f-4ebb-bb54-0ae92c88e64b"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 18, 8, 32, 409, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("5d8949a3-3c3d-44c0-b22e-e9ec1881faf0"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 18, 8, 32, 409, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UnitsOfMeasurements",
                keyColumn: "Id",
                keyValue: new Guid("e6fe6c76-ef68-41dd-be4a-07f46f274334"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 18, 8, 32, 409, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "UomConversionRates",
                keyColumn: "Id",
                keyValue: new Guid("f57c690a-fbb1-47e2-9ab0-1472a514d88f"),
                column: "CreatedOn",
                value: new DateTime(2018, 9, 11, 18, 8, 32, 410, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "PurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "RawCode",
                table: "PurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "RawName",
                table: "PurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "RawQty",
                table: "PurchasingRequestItems");

            migrationBuilder.DropColumn(
                name: "RawUom",
                table: "PurchasingRequestItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "NomenclatureId",
                table: "PurchasingRequestItems",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RawItemId",
                table: "PurchasingRequestItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "RawPurchasingRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    PurchasingRequestId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawPurchasingRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RawPurchasingRequestItems_PurchasingRequests_PurchasingRequestId",
                        column: x => x.PurchasingRequestId,
                        principalTable: "PurchasingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems",
                column: "RawItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RawPurchasingRequestItems_PurchasingRequestId",
                table: "RawPurchasingRequestItems",
                column: "PurchasingRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingRequestItems_RawPurchasingRequestItems_RawItemId",
                table: "PurchasingRequestItems",
                column: "RawItemId",
                principalTable: "RawPurchasingRequestItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

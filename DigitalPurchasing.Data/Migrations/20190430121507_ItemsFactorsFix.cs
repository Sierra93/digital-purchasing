using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ItemsFactorsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NomenclatureFactor",
                table: "SupplierOfferItems",
                type: "decimal(38, 17)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommonFactor",
                table: "SupplierOfferItems",
                type: "decimal(38, 17)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NomenclatureFactor",
                table: "PurchaseRequestItems",
                type: "decimal(38, 17)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommonFactor",
                table: "PurchaseRequestItems",
                type: "decimal(38, 17)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NomenclatureFactor",
                table: "SupplierOfferItems",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommonFactor",
                table: "SupplierOfferItems",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NomenclatureFactor",
                table: "PurchaseRequestItems",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CommonFactor",
                table: "PurchaseRequestItems",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 17)");
        }
    }
}

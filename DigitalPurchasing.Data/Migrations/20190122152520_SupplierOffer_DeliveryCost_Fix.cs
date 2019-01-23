using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierOffer_DeliveryCost_Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryCost",
                table: "SupplierOffers",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryCost",
                table: "SupplierOffers",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");
        }
    }
}

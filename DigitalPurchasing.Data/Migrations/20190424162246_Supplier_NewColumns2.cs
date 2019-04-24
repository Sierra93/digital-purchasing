using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Supplier_NewColumns2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryTerms",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfferCurrency",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentDeferredDays",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierType",
                table: "Suppliers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTerms",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "OfferCurrency",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "PaymentDeferredDays",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SupplierType",
                table: "Suppliers");
        }
    }
}

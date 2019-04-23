using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Supplier_NewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_OwnerId",
                table: "Suppliers");

            migrationBuilder.AddColumn<string>(
                name: "ActualAddressCity",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualAddressCountry",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActualAddressStreet",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErpCode",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inn",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddressCity",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddressCountry",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddressStreet",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnershipType",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseAddressCity",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseAddressCountry",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseAddressStreet",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_OwnerId_Inn",
                table: "Suppliers",
                columns: new[] { "OwnerId", "Inn" },
                unique: true,
                filter: "Name IS NOT NULL AND Inn IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_OwnerId_Inn",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ActualAddressCity",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ActualAddressCountry",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ActualAddressStreet",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ErpCode",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Inn",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "LegalAddressCity",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "LegalAddressCountry",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "LegalAddressStreet",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "OwnershipType",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "WarehouseAddressCity",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "WarehouseAddressCountry",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "WarehouseAddressStreet",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Suppliers");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_OwnerId",
                table: "Suppliers",
                column: "OwnerId");
        }
    }
}

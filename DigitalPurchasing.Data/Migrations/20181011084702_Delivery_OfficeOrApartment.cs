using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class Delivery_OfficeOrApartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apartment",
                table: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "Office",
                table: "Deliveries",
                newName: "OfficeOrApartment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OfficeOrApartment",
                table: "Deliveries",
                newName: "Office");

            migrationBuilder.AddColumn<string>(
                name: "Apartment",
                table: "Deliveries",
                nullable: true);
        }
    }
}

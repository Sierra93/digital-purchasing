using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierContactPerson_Rename_PhoneNumber_To_MobilePhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "SupplierContactPersons",
                newName: "MobilePhoneNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MobilePhoneNumber",
                table: "SupplierContactPersons",
                newName: "PhoneNumber");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SupplierContactPerson_Patronymic_PhoneNumber_JobTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "SupplierContactPersons",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "SupplierContactPersons",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "SupplierContactPersons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "SupplierContactPersons");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "SupplierContactPersons");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "SupplierContactPersons");
        }
    }
}

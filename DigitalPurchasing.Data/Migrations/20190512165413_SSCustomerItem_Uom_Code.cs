using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSCustomerItem_Uom_Code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SSCustomerItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                table: "SSCustomerItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "SSCustomerItems");

            migrationBuilder.DropColumn(
                name: "Uom",
                table: "SSCustomerItems");
        }
    }
}

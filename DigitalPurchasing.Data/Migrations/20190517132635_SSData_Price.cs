using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSData_Price : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "SSDatas",
                type: "decimal(38, 17)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "SSDatas");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSSupplierItem_ConvertedQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConvertedQuantity",
                table: "SSSupplierItems",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertedQuantity",
                table: "SSSupplierItems");
        }
    }
}

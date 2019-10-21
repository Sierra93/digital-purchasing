using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class User_PRDiscountPercentage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PRDiscountPercentage",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 3m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PRDiscountPercentage",
                table: "Users");
        }
    }
}

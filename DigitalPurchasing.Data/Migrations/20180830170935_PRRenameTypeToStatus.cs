using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class PRRenameTypeToStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "PurchasingRequests");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PurchasingRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "PurchasingRequests");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "PurchasingRequests",
                nullable: false,
                defaultValue: 0);
        }
    }
}

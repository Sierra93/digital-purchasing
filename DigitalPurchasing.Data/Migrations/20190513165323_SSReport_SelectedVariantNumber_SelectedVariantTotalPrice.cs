using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class SSReport_SelectedVariantNumber_SelectedVariantTotalPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedVariantNumber",
                table: "SSReports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SelectedVariantTotalPrice",
                table: "SSReports",
                type: "decimal(38, 17)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedVariantNumber",
                table: "SSReports");

            migrationBuilder.DropColumn(
                name: "SelectedVariantTotalPrice",
                table: "SSReports");
        }
    }
}

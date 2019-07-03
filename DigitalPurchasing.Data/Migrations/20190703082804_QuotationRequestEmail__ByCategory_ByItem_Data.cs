using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class QuotationRequestEmail__ByCategory_ByItem_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ByCategory",
                table: "QuotationRequestEmails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ByItem",
                table: "QuotationRequestEmails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "QuotationRequestEmails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByCategory",
                table: "QuotationRequestEmails");

            migrationBuilder.DropColumn(
                name: "ByItem",
                table: "QuotationRequestEmails");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "QuotationRequestEmails");
        }
    }
}

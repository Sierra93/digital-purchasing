using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedEmails_New_Columns3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessingTries",
                table: "ReceivedEmails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessingTries",
                table: "ReceivedEmails");
        }
    }
}

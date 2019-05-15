using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedEmails_New_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "ReceivedEmails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "ReceivedEmails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "ReceivedEmails");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "ReceivedEmails");
        }
    }
}

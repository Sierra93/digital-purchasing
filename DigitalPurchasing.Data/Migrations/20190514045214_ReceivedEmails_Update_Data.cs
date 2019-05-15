using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedEmails_Update_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update dbo.ReceivedEmails set ProcessingTries = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

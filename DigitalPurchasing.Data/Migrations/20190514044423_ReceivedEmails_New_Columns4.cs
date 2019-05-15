using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedEmails_New_Columns4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update dbo.ReceivedEmails set Discriminator = 'ReceivedSoEmail' where Discriminator = 'ReceivedRfqEmail'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update dbo.ReceivedEmails set Discriminator = 'ReceivedRfqEmail' where Discriminator = 'ReceivedSoEmail'");
        }
    }
}

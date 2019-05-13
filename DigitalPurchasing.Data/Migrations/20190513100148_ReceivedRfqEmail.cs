using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedRfqEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ReceivedEmails",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "QuotationRequestId",
                table: "ReceivedEmails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedEmails_QuotationRequestId",
                table: "ReceivedEmails",
                column: "QuotationRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedEmails_QuotationRequests_QuotationRequestId",
                table: "ReceivedEmails",
                column: "QuotationRequestId",
                principalTable: "QuotationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedEmails_QuotationRequests_QuotationRequestId",
                table: "ReceivedEmails");

            migrationBuilder.DropIndex(
                name: "IX_ReceivedEmails_QuotationRequestId",
                table: "ReceivedEmails");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ReceivedEmails");

            migrationBuilder.DropColumn(
                name: "QuotationRequestId",
                table: "ReceivedEmails");
        }
    }
}

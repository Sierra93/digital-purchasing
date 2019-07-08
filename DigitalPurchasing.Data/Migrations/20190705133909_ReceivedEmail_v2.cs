using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class ReceivedEmail_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "ReceivedEmails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToEmail",
                table: "ReceivedEmails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedEmails_OwnerId",
                table: "ReceivedEmails",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedEmails_Companies_OwnerId",
                table: "ReceivedEmails",
                column: "OwnerId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedEmails_Companies_OwnerId",
                table: "ReceivedEmails");

            migrationBuilder.DropIndex(
                name: "IX_ReceivedEmails_OwnerId",
                table: "ReceivedEmails");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ReceivedEmails");

            migrationBuilder.DropColumn(
                name: "ToEmail",
                table: "ReceivedEmails");

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
    }
}

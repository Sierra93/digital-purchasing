using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalPurchasing.Data.Migrations
{
    public partial class QuotationRequestEmail_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "QuotationRequestEmails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequestEmails_UserId",
                table: "QuotationRequestEmails",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationRequestEmails_Users_UserId",
                table: "QuotationRequestEmails",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationRequestEmails_Users_UserId",
                table: "QuotationRequestEmails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationRequestEmails_UserId",
                table: "QuotationRequestEmails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuotationRequestEmails");
        }
    }
}
